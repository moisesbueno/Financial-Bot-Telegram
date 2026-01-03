using Financial.Bot.Dtos;
using Newtonsoft.Json;

public class CoinLoreResponseAll
{
    [JsonProperty("data")]
    public List<CoinLoreResponse> Coins { get; set; }
}