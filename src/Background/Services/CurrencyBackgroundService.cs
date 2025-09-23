using CurrencyTracker.Application.Interfaces;
using System.Xml;

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

    private async Task<Dictionary<string, decimal>> FetchRatesFromCbr()
    {
        var rates = new Dictionary<string, decimal>();

        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("http://www.cbr.ru/scripts/XML_daily.asp");

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            var valuteNodes = xmlDoc.SelectNodes("//Valute");
            if (valuteNodes != null)
            {
                foreach (XmlNode node in valuteNodes)
                {
                    var code = node.SelectSingleNode("CharCode")?.InnerText;
                    var value = node.SelectSingleNode("Value")?.InnerText;
                    var nominal = node.SelectSingleNode("Nominal")?.InnerText;

                    if (code != null && value != null && nominal != null)
                    {
                        // Конвертируем в decimal (заменяем запятую на точку)
                        var rateValue = decimal.Parse(value.Replace(",", ".")) / int.Parse(nominal);
                        rates[code] = rateValue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching rates from CBR");
        }

        return rates;
    }
}