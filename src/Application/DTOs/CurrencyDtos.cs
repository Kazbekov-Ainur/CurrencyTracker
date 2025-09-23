namespace CurrencyTracker.Application.DTOs;
public class CurrencyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class UserCurrencyResponseDto
{
    public UserResponseDto User { get; set; } = new();
    public List<CurrencyDto> FavoriteCurrencies { get; set; } = new();
}