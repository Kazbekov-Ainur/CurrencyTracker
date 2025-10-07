using CurrencyTracker.Domain.Entities;
using CurrencyTracker.Domain.Interfaces;
using CurrencyTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Infrastructure.Repositories;
public class CurrencyRepository : BaseRepository<Currency>, ICurrencyRepository
{
    public CurrencyRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Currency?> GetByCodeAsync(string code)
    {
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task UpdateRatesAsync(Dictionary<string, decimal> rates)
    {
        foreach (var (code, rate) in rates)
        {
            var currency = await GetByCodeAsync(code);
            if (currency != null)
            {
                currency.Rate = rate;
                currency.LastUpdated = DateTime.UtcNow;
                _context.Currencies.Update(currency);
            }
            else
            {
                var newCurrency = new Currency
                {
                    Code = code,
                    Name = code,
                    Rate = rate,
                    LastUpdated = DateTime.UtcNow
                };
                await _context.Currencies.AddAsync(newCurrency);
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Currency>> GetCurrenciesByUserAsync(int userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.FavoriteCurrencies)
            .ToListAsync();
    }
}