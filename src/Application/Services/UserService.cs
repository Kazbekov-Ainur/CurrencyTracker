using CurrencyTracker.Application.DTOs;
using CurrencyTracker.Application.Interfaces;
using CurrencyTracker.Domain.Entities;
using CurrencyTracker.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using BC = BCrypt.Net.BCrypt;

namespace CurrencyTracker.Application.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public UserService(IUserRepository userRepository, ITokenService tokenService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<UserResponseDto> RegisterAsync(UserRegisterDto registerDto)
    {
        // Проверяем, существует ли пользователь
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
            throw new ArgumentException("User with this email already exists");

        // Хешируем пароль
        var passwordHash = BC.HashPassword(registerDto.Password);

        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            Password = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<string> LoginAsync(UserLoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !BC.Verify(loginDto.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid email or password");

        var userDto = new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        return _tokenService.GenerateToken(userDto);
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new ArgumentException("User not found");

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> AddFavoriteCurrencyAsync(int userId, int currencyId)
    {
        return await _userRepository.AddFavoriteCurrencyAsync(userId, currencyId);
    }

    public async Task<bool> RemoveFavoriteCurrencyAsync(int userId, int currencyId)
    {
        return await _userRepository.RemoveFavoriteCurrencyAsync(userId, currencyId);
    }
}