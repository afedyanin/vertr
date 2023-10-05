using Vertr.Exchange.Api;
using Vertr.Exchange.Core;
using Vertr.Exchange.RiskEngine;
using Vertr.Exchange.Accounts;
using Vertr.Exchange.MatchingEngine;
using Vertr.Exchange.Server.Configuration;
using Vertr.Exchange.Server.MessageHandlers;
using Vertr.Exchange.Server.Services;
using Vertr.Exchange.Common.Abstractions;

namespace Vertr.Exchange.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();

        // TODO: Refactor this
        // Register Exchange
        builder.Services.AddExchangeApi();
        builder.Services.AddUserProfileProvider();
        builder.Services.AddOrderRiskEngine();
        builder.Services.AddOrderMatchingEngine();
        builder.Services.UseRiskEngine();
        builder.Services.UseMatchingEngine();

        //builder.Services.AddSingleton<IMessageHandler, LoggingMessageHandler>();
        builder.Services.AddSingleton<IMessageHandler, NatsMessageHandler>();
        builder.Services.AddSingleton<NatsConfiguration>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapGrpcReflectionService();
        app.MapGrpcService<ExchangeApiService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
        app.Run();
    }
}
