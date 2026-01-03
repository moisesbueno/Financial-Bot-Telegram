using Financial.Bot.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Financial.Bot.Services
{
    public class CoinLoreApiClient(IHttpClientFactory httpClientFactory, IOptions<CoinLoreApiOptions> options)
        : ICoinLoreApiClient
    {
        private readonly CoinLoreApiOptions _options = options.Value;

        public async Task<List<CoinLoreResponse>> GetAllCoins(CancellationToken cancellationToken = default)
        {
            using var client = httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri($"{_options.BaseUrl}/tickers/");

            using var response = await client.SendAsync(request, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = JsonConvert.DeserializeObject<CoinLoreResponseAll>(body);

            return response.IsSuccessStatusCode ? result.Coins : Enumerable.Empty<CoinLoreResponse>().ToList();
        }

        public async Task<CoinLoreResponse> GetCoinByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            using var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_options.BaseUrl);

            using var response = await client.GetAsync($"ticker/?id={id}", cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            return !response.IsSuccessStatusCode
                ? null
                : JsonConvert.DeserializeObject<List<CoinLoreResponse>>(body)[0];
        }
    }
}