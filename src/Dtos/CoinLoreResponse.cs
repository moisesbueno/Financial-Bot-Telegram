using Newtonsoft.Json;

namespace Financial.Bot.Dtos
{
    public class CoinLoreResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nameid")]
        public string Nameid { get; set; }

        [JsonProperty("price_usd")]
        public decimal PriceUsd { get; set; }
    }
}
