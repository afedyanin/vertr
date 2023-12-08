using Refit;
using Vertr.Exchange.Terminal.ApiClient;
using Vertr.Exchange.Terminal.ConsoleApp.StaticData;

namespace Vertr.Exchange.Terminal.ConsoleApp;

public class Program
{
    public static async Task Main()
    {
        var exchApi = RestService.For<IHostApiClient>("http://localhost:5010");

        var res = await Commands.Reset(exchApi);
        Console.WriteLine(res);

        res = await Commands.AddSymbols(exchApi);
        Console.WriteLine(res);

        res = await Commands.AddUsers(exchApi);
        Console.WriteLine(res);

        var bobTrading = Task.Run(async () =>
        {
            await TradingStrategy.RandomWalkTrading(exchApi, Users.Bob, Symbols.MSFT, 100m);
        });

        var aliceTrading = Task.Run(async () =>
        {
            await TradingStrategy.RandomWalkTrading(exchApi, Users.Alice, Symbols.MSFT, 100m);
        });

        await Task.WhenAll(aliceTrading, bobTrading);
    }
}
