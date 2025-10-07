using CurrencyTracker.Application.Interfaces;

namespace CurrencyTracker.Background.Services;
public class CurrencyBackgroundService : BackgroundService
{
    private readonly ILogger<CurrencyBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Обновление каждый час

    public CurrencyBackgroundService(ILogger<CurrencyBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();

                await currencyService.UpdateCurrencyRatesAsync();
                _logger.LogInformation("Currency rates updated successfully at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating currency rates at {Time}", DateTime.UtcNow);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}