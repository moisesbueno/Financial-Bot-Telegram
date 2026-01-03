# Financial bot price 
This project consumes third-api and send responses with telegram

### Requirements 

* DOTNET 8 
* Mongo DB
* Docker 
* Telegram bot token

```json
appsettings.Development.json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017/db-logs"
  },
  "CoinLoreApi": {
    "BaseUrl": "https://api.coinlore.net/api/"
  },
  "BotToken": "TOKEN_FROM_TELEGRAM_BOT",
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}

```

<img width="240" height="113" alt="image" src="https://github.com/user-attachments/assets/663522f8-9c80-46ab-8792-d83a5d82be8b" />
