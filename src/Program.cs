using Financial.Bot.HostedServices;
using Financial.Bot.Services;
using Serilog;

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
        builder.Services.AddHttpClient();
        builder.Services.Configure<CoinLoreApiOptions>(builder.Configuration.GetSection("CoinLoreApi"));

        builder.Services.AddTransient<ICoinLoreApiClient, CoinLoreApiClient>();
        builder.Services.AddTransient<CryptoService>();
        builder.Services.AddMemoryCache();

        builder.Services.AddHostedService<TelegramHostedService>();
        builder.Services.AddHostedService<SaveCoinsHostedService>();

        var host = builder.Build();
        host.Run();
    }
}