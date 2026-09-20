using System.ComponentModel.DataAnnotations;

namespace Financial.Bot.Services
{
    public class CoinLoreApiOptions
    {
        public const string CoinLoreApi = "CoinLoreApi";

        [Required]
        public required string  BaseUrl { get; set; }

    }
}