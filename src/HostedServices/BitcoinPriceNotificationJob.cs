using Financial.Bot.Services;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Financial.Bot.Jobs
{
    public class BitcoinPriceNotificationJob(
        ILogger<BitcoinPriceNotificationJob> logger,
        IConfiguration configuration,
        CryptoService cryptoService) : IJob
    {

        private readonly long _chatId = long.Parse(configuration.GetSection("ChatId").Value);
        private readonly string _botToken = configuration.GetSection("BotToken").Value;

        private readonly CryptoService _cryptoService = cryptoService;
        private readonly ILogger<BitcoinPriceNotificationJob> _logger = logger;

        public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var bitcoinId = await _cryptoService.GetCoinIdByNameAsync("bitcoin");
               
                var coinResponse = await _cryptoService.GetCoinByIdAsync(bitcoinId.Value);
                if (coinResponse == null)
                {
                    _logger.LogError("Could not fetch Bitcoin data");
                    return;
                }

                var chatId = new ChatId(_chatId);
                var message = $"""
                                                        <b><u> {coinResponse.Name}</u></b>
                                                        ⚡ Symbol: {coinResponse.Symbol}
                                                        💰 Price : {coinResponse.PriceUsd:C}
                                                        """;

                var botClient = new TelegramBotClient(_botToken);
                await botClient.SendMessage(chatId, message, parseMode: ParseMode.Html, cancellationToken : cancellationToken);
                
                _logger.LogInformation("Bitcoin price notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Bitcoin price notification");
            }
        }
    }
}