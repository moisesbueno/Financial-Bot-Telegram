using System.Globalization;
using Financial.Bot.Services;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Financial.Bot.HostedServices
{
    public class TelegramHostedService(
        ILogger<TelegramHostedService> logger,
        IConfiguration configuration,
        CryptoService cryptoService)
        : IHostedService
    {
        private readonly string _botToken = configuration.GetSection("BotToken").Value;
        private TelegramBotClient _botClient;
        private string _userName;


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient = new TelegramBotClient(token: _botToken, cancellationToken: cancellationToken);

            var me = await _botClient.GetMe(cancellationToken);
            _userName = me.Username;

            _botClient.OnMessage += BotClient_OnMessage;
            _botClient.OnUpdate += BotClient_OnUpdate;
            _botClient.OnError += BotClient_OnError;

            await Task.CompletedTask;
        }

        private async Task BotClient_OnError(Exception exception, Telegram.Bot.Polling.HandleErrorSource source)
        {
            logger.LogError(exception, "Error");
            await Task.CompletedTask;
        }

        private async Task BotClient_OnUpdate(Update update)
        {
            switch (update)
            {
                case { CallbackQuery: { } callbackQuery }: await OnCallbackQuery(callbackQuery); break;
                default: logger.LogInformation("Received unhandled update {Type}", update.Type); break;
            }
            ;
        }

        private async Task BotClient_OnMessage(Message message, UpdateType type)
        {
            if (message.Text is not { } text)
                logger.LogInformation("Received a message of type {Type}", message.Type);
            else if (text.StartsWith('/'))
            {
                var space = text.IndexOf(' ');
                if (space < 0) space = text.Length;
                var command = text[..space].ToLower();
                if (command.LastIndexOf('@') is > 0 and var at) // it's a targeted command
                    if (command[(at + 1)..].Equals(_userName, StringComparison.OrdinalIgnoreCase))
                        command = command[..at];
                    else
                        return; // command was not targeted at me
                await OnCommand(command, text[space..].TrimStart(), message);
            }

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        private async Task OnCommand(string command, string args, Message msg)
        {
            switch (command)
            {
                case "/coins":
                    await SendResponseForCommandGetAllCoins(args, msg);
                    break;
                case "/coin":
                    await SendResponseForCommandGetCoin(args, msg);
                    break;
                case "/bitcoin":
                    await SendResponseForCommandGetCoin("bitcoin", msg);
                    break;
                default:
                    await SendResponseForNotImplemented(args, msg);
                    break;
            }
        }

        private async Task SendResponseForCommandGetAllCoins(string args, Message msg)
        {
            var allCoinsResponse = await cryptoService.GetAllCoins();

            var stringBuilder = new StringBuilder();

            foreach (var item in allCoinsResponse)
            {
                stringBuilder.AppendLine($"⚡{item.Nameid} - {item.Symbol}");
            }

            await _botClient.SendMessage(msg.Chat, stringBuilder.ToString(), parseMode: ParseMode.Html);
            
            using (logger.BeginScope("{UserName}", msg.Chat.Username))
            {
                logger.LogInformation("Message send");
            }
        }


        private async Task OnCallbackQuery(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQuery(callbackQuery.Id, $"You selected {callbackQuery.Data}");
            await _botClient.SendMessage(callbackQuery.Message!.Chat,
                $"Received callback from inline button {callbackQuery.Data}");
        }

        private async Task SendResponseForCommandGetCoin(string args, Message msg)
        {
            int? coinId = await cryptoService.GetCoinIdByNameAsync(args);

            if (!coinId.HasValue)
            {
                await _botClient.SendMessage(msg.Chat, $"🚫 coin not found");
                return;
            }

            var coinResponse = await cryptoService.GetCoinByIdAsync(coinId.Value);

            if (coinResponse is not null)
            {
                await _botClient.SendMessage(msg.Chat, $"""
                                                        <b><u> {coinResponse.Name}</u></b>
                                                        ⚡ Symbol: {coinResponse.Symbol}
                                                        💰 Price : {coinResponse.PriceUsd:C}
                                                        """, parseMode: ParseMode.Html);
            }
            else
            {
                await _botClient.SendMessage(msg.Chat, $"🚫 coin not found");
            }
            
            using (logger.BeginScope("{UserName}", msg.Chat.Username))
            {
                logger.LogInformation("Message send");
            }
        }

        private async Task SendResponseForNotImplemented(string args, Message msg)
        {
            await _botClient.SendMessage(msg.Chat, $"🚫 command not found");
        }
    }
}