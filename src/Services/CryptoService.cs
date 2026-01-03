using Financial.Bot.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Financial.Bot.Services
{
    public class CryptoService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICoinLoreApiClient _coinLoreApiClient;
        private const string FILE = "coins.json";

        public CryptoService(IMemoryCache memoryCache, ICoinLoreApiClient coinLoreApiClient)
        {
            _memoryCache = memoryCache;
            _coinLoreApiClient = coinLoreApiClient;
        }

        public async Task<CoinLoreResponse> GetCoinByIdAsync(int coinId, CancellationToken cancellationToken = default)
        {
            var cacheResult = await _memoryCache.GetOrCreateAsync($"coin-{coinId}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15);

                return _coinLoreApiClient.GetCoinByIdAsync(coinId, cancellationToken);
            });

            return cacheResult;
        }

        public async Task<List<CoinLoreResponse>> GetAllCoins(CancellationToken cancellationToken = default)
        {
            var cacheResult = await _memoryCache.GetOrCreateAsync("all-coins", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                return await GetAllCoinsFromFile(cancellationToken);
            });

            return cacheResult;
        }

        public async Task SaveCoinsOnFile(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(FILE))
            {
                var coins = await _coinLoreApiClient.GetAllCoins(cancellationToken);

                var coinsInformation = coins.Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Nameid,
                        c.Symbol,
                    })
                    .OrderBy(c => c.Id)
                    .ToList();

                var json = JsonConvert.SerializeObject(coinsInformation, Formatting.Indented);

                await File.WriteAllTextAsync(FILE, json, cancellationToken);
            }
        }

        public async Task<int?> GetCoinIdByNameAsync(string coinName, CancellationToken cancellationToken = default)
        {
            var allCoins = await GetAllCoinsFromFile(cancellationToken);

            var coin = allCoins.FirstOrDefault(c => c.Name.Equals(coinName, StringComparison.OrdinalIgnoreCase) ||
                                                    c.Symbol.Equals(coinName, StringComparison.OrdinalIgnoreCase));

            return coin?.Id;
        }

        public async Task<List<CoinLoreResponse>> GetAllCoinsFromFile(CancellationToken cancellationToken = default)
        {
            var file = await File.ReadAllTextAsync(FILE, cancellationToken);

            var allCoins = JsonConvert.DeserializeObject<List<CoinLoreResponse>>(file);

            return allCoins;
        }
    }
}