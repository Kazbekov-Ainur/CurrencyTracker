using CurrencyTracker.Application.DTOs;

namespace CurrencyTracker.Application.Interfaces;
public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto);
    Task<string> LoginAsync(UserLoginDto loginDto);
    Task<UserResponseDto> GetUserByIdAsync(int id);
    Task<bool> AddFavoriteCurrencyAsync(int userId, int currencyId);
    Task<bool> RemoveFavoriteCurrencyAsync(int userId, int currencyId);
}