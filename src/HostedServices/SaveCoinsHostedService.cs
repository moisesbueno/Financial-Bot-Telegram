using Financial.Bot.Services;
using Microsoft.Extensions.Hosting;

public class SaveCoinsHostedService(CryptoService cryptoService) : IHostedService
{
    private readonly CryptoService _cryptoService = cryptoService;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _cryptoService.SaveCoinsOnFile(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}