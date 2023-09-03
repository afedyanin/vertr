using Vertr.Exchange.Common.Abstractions;
using Vertr.Exchange.Common.Binary.Commands;
using Vertr.Exchange.Common.Enums;
using Vertr.Exchange.RiskEngine.Symbols;

namespace Vertr.Exchange.RiskEngine.Binary;

internal static class CommandExtensions
{
    public static CommandResultCode HandleCommand(
        this BatchAddSymbolsCommand command,
        ISymbolSpecificationProvider symbolSpecificationProvider)
    {
        symbolSpecificationProvider.AddSymbols(command.Symbols);
        return CommandResultCode.SUCCESS;
    }

    public static CommandResultCode HandleCommand(
        this BatchAddAccountsCommand command,
        IUserProfileProvider userProfiles)
    {
        userProfiles.BatchAdd(command.Users);
        return CommandResultCode.SUCCESS;
    }
}
