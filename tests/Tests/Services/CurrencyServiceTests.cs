using CurrencyTracker.Application.DTOs;
using CurrencyTracker.Application.Services;
using CurrencyTracker.Domain.Entities;
using CurrencyTracker.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyTracker.Tests.Services;
public class CurrencyServiceTests
{
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CurrencyService _currencyService;
    private readonly Mock<ILogger<CurrencyService>> _loggerMock;

    public CurrencyServiceTests()
    {
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<CurrencyService>>();
        _currencyService = new CurrencyService(_currencyRepositoryMock.Object, _userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetUserCurrenciesAsync_ShouldReturnUserCurrencies_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var user = new User { Id = userId, Name = "Test User", Email = "test@example.com" };
        var currencies = new List<Currency>
        {
            new Currency { Id = 1, Name = "US Dollar", Code = "USD", Rate = 90.5m },
            new Currency { Id = 2, Name = "Euro", Code = "EUR", Rate = 98.2m }
        };

        _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _currencyRepositoryMock.Setup(x => x.GetCurrenciesByUserAsync(userId))
            .ReturnsAsync(currencies);

        // Act
        var result = await _currencyService.GetUserCurrenciesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.User.Id);
        Assert.Equal(2, result.FavoriteCurrencies.Count);
    }

    [Fact]
    public async Task GetAllCurrenciesAsync_ShouldReturnAllCurrencies()
    {
        // Arrange
        var currencies = new List<Currency>
        {
            new Currency { Id = 1, Name = "US Dollar", Code = "USD", Rate = 90.5m },
            new Currency { Id = 2, Name = "Euro", Code = "EUR", Rate = 98.2m }
        };

        _currencyRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(currencies);

        // Act
        var result = await _currencyService.GetAllCurrenciesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}