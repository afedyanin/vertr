using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Exchange.SignalRClient.Awaiting;
using Vertr.Exchange.SignalRClient.Configuration;
using Vertr.Exchange.SignalRClient.Providers;
using Vertr.Exchange.SignalRClient.Streams;

namespace Vertr.Exchange.SignalRClient;

public static class ExchangeApiClientRegistrar
{
    public static IServiceCollection AddExchangeApi(this IServiceCollection services, IConfiguration configuration)
    {
        var exchangeOptions = new ExchangeConfiguration();
        configuration.GetSection(ExchangeConfiguration.SectionName).Bind(exchangeOptions);

        services.AddSingleton<IHubConnectionProvider, HubConnectionProvider>();
        services.AddSingleton<ICommandAwaitingService, CommandAwaitingService>();
        services.AddSingleton<IExchangeApiClient, ExchangeApiClient>();

        services.AddHostedService<CommandResultStream>();
        services.AddHostedService<OrderBooksStream>();
        services.AddHostedService<TradeEventStream>();
        services.AddHostedService<ReduceEventStream>();
        services.AddHostedService<RejectEventStream>();

        return services;
    }
}
