using CurrencyTracker.Application.DTOs;

namespace CurrencyTracker.Application.Interfaces;
public interface ITokenService
{
    string GenerateToken(UserResponseDto user);
    bool ValidateToken(string token);
    int GetUserIdFromToken(string token);
}