using CurrencyTracker.Application.DTOs;

namespace CurrencyTracker.Application.Interfaces;
public interface ICurrencyService
{
    Task UpdateCurrencyRatesAsync();
    Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
    Task<UserCurrencyResponseDto> GetUserCurrenciesAsync(int userId);
}