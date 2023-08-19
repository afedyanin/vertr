using System.Diagnostics;
using Vertr.Exchange.Common;
using Vertr.Exchange.Common.Abstractions;
using Vertr.Exchange.Common.Enums;
using Vertr.Exchange.Common.Symbols;
using Vertr.Exchange.RiskEngine.Abstractions;
using Vertr.Exchange.RiskEngine.Extensions;
using Vertr.Exchange.RiskEngine.Users;

namespace Vertr.Exchange.RiskEngine.Orders;

internal class PostProcessOrderCommand
{
    private readonly OrderCommand _orderCommand;
    private readonly IOrderRiskEngineInternal _orderRiskEngine;

    private IUserProfileService _userProfileService
        => _orderRiskEngine.UserProfileService;

    private ISymbolSpecificationProvider _symbolSpecificationProvider
        => _orderRiskEngine.SymbolSpecificationProvider;

    private IFeeCalculationService _feeCalculationService
        => _orderRiskEngine.FeeCalculationService;

    private ILastPriceCacheProvider _lastPriceCacheProvider
        => _orderRiskEngine.LastPriceCacheProvider;

    public PostProcessOrderCommand(
        IOrderRiskEngineInternal orderRiskEngine,
        OrderCommand command)
    {
        _orderCommand = command;
        _orderRiskEngine = orderRiskEngine;
    }
    public bool Execute()
    {
        var symbol = _orderCommand.Symbol;
        var marketData = _orderCommand.MarketData;
        var mte = _orderCommand.MatcherEvent;

        // skip events processing if no events (or if contains BINARY EVENT)
        if (marketData == null && (mte == null || mte.EventType == MatcherEventType.BINARY_EVENT))
        {
            return false;
        }

        var spec = _symbolSpecificationProvider.GetSymbolSpecification(symbol) ??
            throw new InvalidOperationException("Symbol not found: " + symbol);

        var takerSell = _orderCommand.Action == OrderAction.ASK;

        if (mte != null && mte.EventType != MatcherEventType.BINARY_EVENT)
        {
            // at least one event to process, resolving primary/taker user profile
            // TODO processing order is reversed
            if (spec.Type == SymbolType.CURRENCY_EXCHANGE_PAIR)
            {
                var takerUp = _userProfileService.GetUserProfileOrAddSuspended(_orderCommand.Uid);

                // REJECT always comes first; REDUCE is always single event
                if (mte.EventType is MatcherEventType.REDUCE or MatcherEventType.REJECT)
                {
                    if (takerUp != null)
                    {
                        HandleMatcherRejectReduceEventExchange(_orderCommand, mte, spec, takerSell, takerUp);
                    }
                    mte = mte.NextEvent;
                }

                if (mte != null)
                {
                    if (takerSell)
                    {
                        HandleMatcherEventsExchangeSell(mte, spec, takerUp!);
                    }
                    else
                    {
                        HandleMatcherEventsExchangeBuy(mte, spec, takerUp!, _orderCommand);
                    }
                }
            }
            else
            {

                var takerUp = _userProfileService.GetUserProfileOrAddSuspended(_orderCommand.Uid);

                // for margin-mode symbols also resolve position record
                var takerSpr = takerUp?.GetCurrentPosition(symbol);
                do
                {
                    handleMatcherEventMargin(mte, spec, _orderCommand.Action!.Value, takerUp!, takerSpr!);
                    mte = mte.NextEvent;
                } while (mte != null);
            }
        }

        // Process marked data
        if (marketData is not null && _orderRiskEngine.IsMarginTradingEnabled)
        {
            // TODO: Check sorting
            var askPrice = marketData.AskSize != 0 ? marketData.AskPrices[0] : long.MaxValue;
            var bidPrice = marketData.BidSize != 0 ? marketData.BidPrices[0] : 0;
            _lastPriceCacheProvider.AddLastPrice(symbol, askPrice, bidPrice);
        }

        return false;
    }

    private void handleMatcherEventMargin(
        IMatcherTradeEvent ev,
        CoreSymbolSpecification spec,
        OrderAction takerAction,
        UserProfile takerUp,
        SymbolPositionRecord takerSpr)
    {
        if (takerUp != null)
        {
            if (ev.EventType == MatcherEventType.TRADE)
            {
                // update taker's position
                var sizeOpen = takerSpr.UpdatePositionForMarginTrade(takerAction, ev.Size, ev.Price);
                var fee = spec.TakerFee * sizeOpen;
                takerUp.AddToValue(spec.QuoteCurrency, -fee);

                _feeCalculationService.AddFeeValue(spec.QuoteCurrency, fee);
            }
            else if (ev.EventType is MatcherEventType.REJECT or MatcherEventType.REDUCE)
            {
                // for cancel/rejection only one party is involved
                takerSpr.PendingRelease(takerAction, ev.Size);
            }

            if (takerSpr.IsEmpty())
            {
                takerUp.RemovePositionRecord(takerSpr);
            }
        }

        if (ev.EventType == MatcherEventType.TRADE)
        {
            // update maker's position
            var maker = _userProfileService.GetUserProfileOrAddSuspended(ev.MatchedOrderUid);
            var makerSpr = maker.GetCurrentPosition(spec.SymbolId);
            var sizeOpen = makerSpr!.UpdatePositionForMarginTrade(GetOppositeAction(takerAction), ev.Size, ev.Price);
            var fee = spec.MakerFee * sizeOpen;
            maker.AddToValue(spec.QuoteCurrency, -fee);

            _feeCalculationService.AddFeeValue(spec.QuoteCurrency, fee);

            if (makerSpr.IsEmpty())
            {
                maker.RemovePositionRecord(makerSpr);
            }
        }
    }

    private void HandleMatcherRejectReduceEventExchange(
        OrderCommand cmd,
        IMatcherTradeEvent ev,
        CoreSymbolSpecification spec,
        bool takerSell,
        UserProfile taker)
    {
        //log.debug("REDUCE/REJECT {} {}", cmd, ev);

        // for cancel/rejection only one party is involved
        if (takerSell)
        {
            taker.AddToValue(spec.BaseCurrency, spec.CalculateAmountAsk(ev.Size));
        }
        else
        {
            if (cmd.Command == OrderCommandType.PLACE_ORDER && cmd.OrderType == OrderType.FOK_BUDGET)
            {
                taker.AddToValue(spec.QuoteCurrency, spec.CalculateAmountBidTakerFeeForBudget(ev.Size, ev.Price));
            }
            else
            {
                taker.AddToValue(spec.QuoteCurrency, spec.CalculateAmountBidTakerFee(ev.Size, ev.BidderHoldPrice));
            }
            // TODO for OrderType.IOC_BUDGET - for REJECT should release leftover deposit after all trades calculated
        }
    }

    private void HandleMatcherEventsExchangeSell(
        IMatcherTradeEvent ev,
        CoreSymbolSpecification spec,
        UserProfile taker)
    {
        //log.debug("TRADE EXCH SELL {}", ev);

        var takerSizeForThisHandler = decimal.Zero;
        var makerSizeForThisHandler = decimal.Zero;
        var takerSizePriceForThisHandler = decimal.Zero;
        var quoteCurrency = spec.QuoteCurrency;

        while (ev != null)
        {
            Debug.Assert(ev.EventType == MatcherEventType.TRADE);

            // aggregate transfers for selling taker
            if (taker != null)
            {
                takerSizePriceForThisHandler += ev.Size * ev.Price;
                takerSizeForThisHandler += ev.Size;
            }

            var size = ev.Size;
            var maker = _userProfileService.GetUserProfileOrAddSuspended(ev.MatchedOrderUid);

            // buying, use bidderHoldPrice to calculate released amount based on price difference
            var priceDiff = ev.BidderHoldPrice - ev.Price;
            var amountDiffToReleaseInQuoteCurrency = spec.CalculateAmountBidReleaseCorrMaker(size, priceDiff);
            maker.AddToValue(quoteCurrency, amountDiffToReleaseInQuoteCurrency);

            var gainedAmountInBaseCurrency = spec.CalculateAmountAsk(size);
            maker.AddToValue(spec.BaseCurrency, gainedAmountInBaseCurrency);
            makerSizeForThisHandler += size;

            ev = ev.NextEvent!;
        }

        taker?.AddToValue(quoteCurrency, (takerSizePriceForThisHandler * spec.QuoteScaleK) - (spec.TakerFee * takerSizeForThisHandler));

        if (takerSizeForThisHandler != 0 || makerSizeForThisHandler != 0)
        {
            _feeCalculationService.AddFeeValue(quoteCurrency, (spec.TakerFee * takerSizeForThisHandler) + (spec.MakerFee * makerSizeForThisHandler));
        }
    }

    private void HandleMatcherEventsExchangeBuy(
        IMatcherTradeEvent ev,
        CoreSymbolSpecification spec,
        UserProfile taker,
        OrderCommand cmd)
    {
        //log.debug("TRADE EXCH BUY {}", ev);

        var takerSizeForThisHandler = decimal.Zero;
        var makerSizeForThisHandler = decimal.Zero;
        var takerSizePriceSum = decimal.Zero;
        var takerSizePriceHeldSum = decimal.Zero;

        var quoteCurrency = spec.QuoteCurrency;

        while (ev != null)
        {
            Debug.Assert(ev.EventType == MatcherEventType.TRADE);

            // perform transfers for taker
            if (taker != null)
            {

                takerSizePriceSum += ev.Size * ev.Price;
                takerSizePriceHeldSum += ev.Size * ev.BidderHoldPrice;
                takerSizeForThisHandler += ev.Size;
            }

            var size = ev.Size;
            var maker = _userProfileService.GetUserProfileOrAddSuspended(ev.MatchedOrderUid);
            var gainedAmountInQuoteCurrency = spec.CalculateAmountBid(size, ev.Price);
            maker.AddToValue(quoteCurrency, gainedAmountInQuoteCurrency - (spec.MakerFee * size));
            makerSizeForThisHandler += size;

            ev = ev.NextEvent!;
        }

        if (taker != null)
        {

            if (cmd.Command == OrderCommandType.PLACE_ORDER && cmd.OrderType == OrderType.FOK_BUDGET)
            {
                // for FOK budget held sum calculated differently
                takerSizePriceHeldSum = cmd.Price;
            }
            // TODO IOC_BUDGET - order can be partially rejected - need held taker fee correction

            taker.AddToValue(quoteCurrency, (takerSizePriceHeldSum - takerSizePriceSum) * spec.QuoteScaleK);
            taker.AddToValue(spec.BaseCurrency, takerSizeForThisHandler * spec.BaseScaleK);
        }

        if (takerSizeForThisHandler != 0 || makerSizeForThisHandler != 0)
        {
            _feeCalculationService.AddFeeValue(quoteCurrency, (spec.TakerFee * takerSizeForThisHandler) + (spec.MakerFee * makerSizeForThisHandler));
        }
    }

    private OrderAction GetOppositeAction(OrderAction action)
        => action == OrderAction.ASK ? OrderAction.BID : OrderAction.ASK;
}
