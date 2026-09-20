using Financial.Bot.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Financial.Bot.Services
{
    public class CoinLoreApiClient(HttpClient httpClient) : ICoinLoreApiClient
{
    public async Task<List<CoinLoreResponse>> GetAllCoins(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("tickers/", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return [];

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonConvert.DeserializeObject<CoinLoreResponseAll>(body)?.Coins ?? [];
    }

    public async Task<CoinLoreResponse> GetCoinByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"ticker/?id={id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonConvert.DeserializeObject<List<CoinLoreResponse>>(body)?.FirstOrDefault();
    }
}
}