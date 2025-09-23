using CurrencyTracker.Domain.Entities;

namespace CurrencyTracker.Domain.Interfaces;
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByNameAsync(string name);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> AddFavoriteCurrencyAsync(int userId, int currencyId);
    Task<bool> RemoveFavoriteCurrencyAsync(int userId, int currencyId);
}