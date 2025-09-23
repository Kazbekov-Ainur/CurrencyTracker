using CurrencyTracker.Domain.Entities;

namespace CurrencyTracker.Domain.Interfaces;
public interface ICurrencyRepository : IRepository<Currency>
{
    Task<Currency?> GetByCodeAsync(string code);
    Task UpdateRatesAsync(Dictionary<string, decimal> rates);
    Task<IEnumerable<Currency>> GetCurrenciesByUserAsync(int userId);
}