using CurrencyTracker.Application.DTOs;
using CurrencyTracker.Application.Interfaces;
using CurrencyTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Xml;

namespace CurrencyTracker.Application.Services;
public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CurrencyService> _logger;

    public CurrencyService(ICurrencyRepository currencyRepository, IUserRepository userRepository, ILogger<CurrencyService> logger)
    {
        _currencyRepository = currencyRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task UpdateCurrencyRatesAsync()
    {
        // Этот метод будет вызываться из фонового сервиса
        var rates = await FetchRatesFromCbr();
        await _currencyRepository.UpdateRatesAsync(rates);
    }

    public async Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync()
    {
        var currencies = await _currencyRepository.GetAllAsync();
        return currencies.Select(c => new CurrencyDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            Rate = c.Rate,
            LastUpdated = c.LastUpdated
        });
    }

    public async Task<UserCurrencyResponseDto> GetUserCurrenciesAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        var currencies = await _currencyRepository.GetCurrenciesByUserAsync(userId);

        var userDto = new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        var currencyDtos = currencies.Select(c => new CurrencyDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            Rate = c.Rate,
            LastUpdated = c.LastUpdated
        }).ToList();

        return new UserCurrencyResponseDto
        {
            User = userDto,
            FavoriteCurrencies = currencyDtos
        };
    }

    private async Task<Dictionary<string, decimal>> FetchRatesFromCbr()
    {
        var rates = new Dictionary<string, decimal>();
        try
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("http://www.cbr.ru/scripts/XML_daily.asp");
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.GetEncoding("windows-1251"));

            var xmlContent = await reader.ReadToEndAsync();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

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
                        var rateValue = decimal.Parse(value.Replace(",", "."), CultureInfo.InvariantCulture) / int.Parse(nominal);
                        rates[code] = rateValue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error fetching rates from CBR");
        }
        return rates;
    }
}