using CurrencyTracker.Domain.Entities;
using CurrencyTracker.Domain.Interfaces;
using CurrencyTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTracker.Infrastructure.Repositories;
public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByNameAsync(string name)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.FavoriteCurrencies)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> AddFavoriteCurrencyAsync(int userId, int currencyId)
    {
        var user = await _context.Users
            .Include(u => u.FavoriteCurrencies)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var currency = await _context.Currencies.FindAsync(currencyId);

        if (user == null || currency == null)
            return false;

        if (!user.FavoriteCurrencies.Any(c => c.Id == currencyId))
        {
            user.FavoriteCurrencies.Add(currency);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveFavoriteCurrencyAsync(int userId, int currencyId)
    {
        var user = await _context.Users
            .Include(u => u.FavoriteCurrencies)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var currency = user?.FavoriteCurrencies.FirstOrDefault(c => c.Id == currencyId);
        if (currency != null)
        {
            user!.FavoriteCurrencies.Remove(currency);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}