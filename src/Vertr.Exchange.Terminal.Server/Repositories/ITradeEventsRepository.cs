using Vertr.Exchange.Contracts;

namespace Vertr.Exchange.Terminal.Server.Repositories;

public interface ITradeEventsRepository
{
    Task<bool> Save(TradeEvent orderBook);

    Task<TradeEvent[]> GetList();
}
