using CurrencyTracker.Application.DTOs;
using CurrencyTracker.Application.Interfaces;
using CurrencyTracker.Domain.Interfaces;

namespace CurrencyTracker.Application.Services;
public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserRepository _userRepository;

    public CurrencyService(ICurrencyRepository currencyRepository, IUserRepository userRepository)
    {
        _currencyRepository = currencyRepository;
        _userRepository = userRepository;
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
        // Временная реализация - полная реализация будет в фоновом сервисе
        return new Dictionary<string, decimal>
        {
            ["USD"] = 90.5m,
            ["EUR"] = 98.2m
        };
    }
}