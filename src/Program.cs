using Financial.Bot.HostedServices;
using Financial.Bot.Services;
using Microsoft.AspNetCore.Builder;
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

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.MongoDB(builder.Configuration.GetConnectionString("MongoDb"), "logs")
                .CreateLogger();

            logging.ClearProviders();
            logging.AddSerilog();
        });
        builder.Services.AddHttpClient();
        builder.Services.Configure<CoinLoreApiOptions>(builder.Configuration.GetSection("CoinLoreApi"));
       
        builder.Services.AddTransient<ICoinLoreApiClient, CoinLoreApiClient>();
        builder.Services.AddTransient<CryptoService>();
        builder.Services.AddMemoryCache();

        builder.Services.AddHostedService<TelegramHostedService>();
        builder.Services.AddHostedService<SaveCoinsHostedService>();

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.SetDefaultCulture("en-US");
        });

        var host = builder.Build();
        host.Run();
    }
}