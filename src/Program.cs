using Financial.Bot.HostedServices;
using Financial.Bot.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Polly;

namespace Financial.Bot;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();

            var loggerConfiguration = new LoggerConfiguration()
                                       .MinimumLevel.Information()
                                       .WriteTo.Console();

            var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb");

            if (!string.IsNullOrEmpty(mongoConnectionString))
            {
                loggerConfiguration = loggerConfiguration.WriteTo.MongoDB(mongoConnectionString, "logs");
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            logging.ClearProviders();
            logging.AddSerilog(Log.Logger, true);
        });

        builder.Services.AddOptions<CoinLoreApiOptions>()
                        .BindConfiguration(CoinLoreApiOptions.CoinLoreApi)
                        .ValidateOnStart();

        builder.Services.AddHttpClient<ICoinLoreApiClient, CoinLoreApiClient>((sp, client) =>
                        {
                            var options = sp.GetRequiredService<IOptions<CoinLoreApiOptions>>().Value;
                            client.BaseAddress = new Uri(options.BaseUrl);
                        })
                        .AddStandardResilienceHandler(options =>
                        {
                            options.Retry.MaxRetryAttempts = 3;
                            options.Retry.BackoffType = DelayBackoffType.Exponential;
                            options.Retry.UseJitter = true;
                            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
                            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
                        });

        builder.Services.AddTransient<CryptoService>();
        builder.Services.AddMemoryCache();

        builder.Services.AddHostedService<TelegramHostedService>();
        builder.Services.AddHostedService<SaveCoinsHostedService>();

        var host = builder.Build();
        host.Run();
    }
}