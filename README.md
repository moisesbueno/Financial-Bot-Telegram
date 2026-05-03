# Financial bot price 
This project consumes CoinLore API and sends cryptocurrency price responses via Telegram bot.

### How to run ?
```console
dotnet run
or
docker compose up --build
```
### Requirements 

* DOTNET 10
* Mongo DB (Optional)
* Docker (Optional)
* Telegram bot token

### Configuration

Before running the application, you need to set up the configuration in `appsettings.Development.json` or `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017/db-logs"
  },
  "CoinLoreApi": {
    "BaseUrl": "https://api.coinlore.net/api/"
  },
  "BotToken": "YOUR_TELEGRAM_BOT_TOKEN_HERE",
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

### Telegram Commands

The bot supports the following commands:
- `/coins` - Lists all supported coins
- `/coin <coin-name>` - Get price for specific coin
- `/bitcoin` - Get Bitcoin price

### Caching
- Coins list cached for 1 minute
- Individual coin prices cached for 15 seconds

### Architecture
- Entry point: `Program.cs`
- Main services:
  - `TelegramHostedService` - Handles Telegram bot interactions
  - `CryptoService` - Manages coin data and caching
  - `CoinLoreApiClient` - Communicates with CoinLore API
### Requirements 

* DOTNET 10
* Mongo DB (Optional)
* Docker (Optional)
* Telegram bot token

<img width="240" height="113" alt="image" src="https://github.com/user-attachments/assets/663522f8-9c80-46ab-8792-d83a5d82be8b" />
<img width="721" height="139" alt="image" src="https://github.com/user-attachments/assets/78d7eda3-02d3-46cd-81d6-d6125f916126" />

