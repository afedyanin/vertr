using Vertr.Exchange.Api.Commands;
using Vertr.Exchange.Api.Tests.Stubs;
using Vertr.Exchange.Common.Enums;

namespace Vertr.Exchange.Api.Tests.Commands;

[TestFixture(Category = "Unit")]
public class AddSymbolsCommandTests : CommandTestBase
{
    [Test]
    public async Task CanAddSymbols()
    {
        var cmd = new AddSymbolsCommand(1L, DateTime.UtcNow, SymbolSpecificationStub.GetSymbols);
        var res = await Api.SendAsync(cmd);
        Assert.That(res.ResultCode, Is.EqualTo(CommandResultCode.SUCCESS));
    }

    [Test]
    public async Task CannotAddSymbolWithTheSameId()
    {
        var cmd = new AddSymbolsCommand(1L, DateTime.UtcNow, SymbolSpecificationStub.GetSymbols);
        var res = await Api.SendAsync(cmd);
        Assert.That(res.ResultCode, Is.EqualTo(CommandResultCode.SUCCESS));

        var cmd2 = new AddSymbolsCommand(2L, DateTime.UtcNow, SymbolSpecificationStub.GetSymbols);
        var res2 = await Api.SendAsync(cmd2);
        Assert.That(res2.ResultCode, Is.EqualTo(CommandResultCode.SYMBOL_MGMT_SYMBOL_ALREADY_EXISTS));
    }
}
