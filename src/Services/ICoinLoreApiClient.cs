using Financial.Bot.Dtos;

namespace Financial.Bot.Services
{
    public interface ICoinLoreApiClient
    {
        public Task<List<CoinLoreResponse>> GetAllCoins(CancellationToken cancellationToken = default);

        public Task<CoinLoreResponse> GetCoinByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}