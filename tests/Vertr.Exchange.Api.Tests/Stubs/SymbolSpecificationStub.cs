using Vertr.Exchange.Common;
using Vertr.Exchange.Common.Enums;

namespace Vertr.Exchange.Api.Tests.Stubs;
internal static class SymbolSpecificationStub
{
    public static SymbolSpecification[] GetSymbols
        => new SymbolSpecification[]
        {
            new SymbolSpecification
            {
                Type = SymbolType.CURRENCY_EXCHANGE_PAIR,
                SymbolId = 1,
                Currency = 10,
            },
            new SymbolSpecification
            {
                Type = SymbolType.FUTURES_CONTRACT,
                SymbolId = 2,
                Currency = 20,
            },
            new SymbolSpecification
            {
                Type = SymbolType.OPTION,
                SymbolId = 3,
                Currency = 30,
            },
            new SymbolSpecification
            {
                Type = SymbolType.EQUITY,
                SymbolId = 4,
                Currency = 40,
            },
        };
}
