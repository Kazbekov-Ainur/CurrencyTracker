using Castle.Core.Configuration;
using CurrencyTracker.Application.DTOs;
using CurrencyTracker.Application.Interfaces;
using CurrencyTracker.Application.Services;
using CurrencyTracker.Domain.Entities;
using CurrencyTracker.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace CurrencyTracker.Tests.Services;
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();

        var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:Key", "SuperSecretKeyForTesting123!"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _configuration);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnUser_WhenValidData()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((User)null);

        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(registerDto.Name, result.Name);
        Assert.Equal(registerDto.Email, result.Email);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenUserExists()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "password123"
        };

        var existingUser = new User { Email = registerDto.Email };
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _userService.RegisterAsync(registerDto));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Email = loginDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(loginDto.Password)
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<UserResponseDto>()))
            .Returns("test_token");

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        Assert.Equal("test_token", result);
    }
}