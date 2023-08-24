using Vertr.Exchange.Common.Enums;
using Vertr.Exchange.Common;
using Vertr.Exchange.Common.Binary;
using Vertr.Exchange.Common.Abstractions;
using System.Runtime.CompilerServices;
using Vertr.Exchange.RiskEngine.Orders;
using Vertr.Exchange.Accounts.Abstractions;
using Vertr.Exchange.Accounts.UserCommands;
using Vertr.Exchange.RiskEngine.Symbols;

[assembly: InternalsVisibleTo("Vertr.Exchange.RiskEngine.Tests")]

namespace Vertr.Exchange.RiskEngine;
internal sealed class OrderRiskEngine : IOrderRiskEngine
{
    public IUserProfilesRepository UserProfiles { get; }

    public ISymbolSpecificationProvider SymbolSpecificationProvider { get; }

    public OrderRiskEngine(
        IUserProfilesRepository userProfiles,
        ISymbolSpecificationProvider symbolSpecificationProvider)
    {
        UserProfiles = userProfiles;
        SymbolSpecificationProvider = symbolSpecificationProvider;
    }

    public bool PreProcessCommand(long seq, OrderCommand cmd)
    {
        switch (cmd.Command)
        {
            case OrderCommandType.PLACE_ORDER:
                var handler = new PreProcessOrderHandler(UserProfiles, SymbolSpecificationProvider);
                cmd.ResultCode = handler.Handle(cmd);
                return false;

            case OrderCommandType.ADD_USER:
            case OrderCommandType.SUSPEND_USER:
            case OrderCommandType.RESUME_USER:
            case OrderCommandType.BALANCE_ADJUSTMENT:
                var userCommand = UserCommandFactory.CreateUserCommand(cmd, UserProfiles);
                cmd.ResultCode = userCommand.Execute();
                return false;

            case OrderCommandType.BINARY_DATA_COMMAND:
            case OrderCommandType.BINARY_DATA_QUERY:
                // ignore return result, because it should be set by MatchingEngineRouter
                var _ = AcceptBinaryCommand(cmd);
                cmd.ResultCode = CommandResultCode.VALID_FOR_MATCHING_ENGINE;
                return false;

            case OrderCommandType.RESET:
                Reset();
                cmd.ResultCode = CommandResultCode.SUCCESS;
                return false;

            case OrderCommandType.MOVE_ORDER:
            case OrderCommandType.CANCEL_ORDER:
            case OrderCommandType.REDUCE_ORDER:
            case OrderCommandType.ORDER_BOOK_REQUEST:
            case OrderCommandType.PERSIST_STATE_MATCHING:
            case OrderCommandType.PERSIST_STATE_RISK:
            case OrderCommandType.GROUPING_CONTROL:
            case OrderCommandType.NOP:
            case OrderCommandType.SHUTDOWN_SIGNAL:
            case OrderCommandType.RESERVED_COMPRESSED:
            default:
                break;
        }
        return false;
    }

    public bool PostProcessCommand(long seq, OrderCommand cmd)
    {
        var handler = new PostProcessOrderHandler(UserProfiles, SymbolSpecificationProvider);
        return handler.Handle(cmd);
    }

    private void Reset()
    {
        UserProfiles.Reset();
        SymbolSpecificationProvider.Reset();
    }

    private CommandResultCode AcceptBinaryCommand(OrderCommand cmd)
    {
        if (cmd.Command is OrderCommandType.BINARY_DATA_COMMAND)
        {
            var command = BinaryCommandFactory.GetBinaryCommand(cmd.BinaryCommandType, cmd.BinaryData);

            if (command != null)
            {
                HandleBinaryCommand(command);
            }
        }

        return CommandResultCode.SUCCESS;
    }

    private void HandleBinaryCommand(IBinaryCommand binCmd)
    {
        if (binCmd is BatchAddSymbolsCommand batchAddSymbolsCommand)
        {
            SymbolSpecificationProvider.AddSymbols(batchAddSymbolsCommand.Symbols);
        }
        else if (binCmd is BatchAddAccountsCommand batchAddAccountsCommand)
        {
            UserProfiles.BatchAdd(batchAddAccountsCommand.Users);
        }
    }
}
