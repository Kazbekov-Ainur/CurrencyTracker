using CurrencyTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace CurrencyTracker.Finance.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceController : ControllerBase
{
    private readonly ICurrencyService _currencyService;
    private readonly ITokenService _tokenService;

    public FinanceController(ICurrencyService currencyService, ITokenService tokenService)
    {
        _currencyService = currencyService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Получение списка валют, отслеживаемых текущим пользователем (избранные).
    /// </summary>
    /// <returns>Список избранных валют пользователя.</returns>
    [HttpGet("user-currencies")]
    [SwaggerOperation(Summary = "Получение списка избранных валют пользователя")]
    public async Task<IActionResult> GetUserCurrencies()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var userCurrencies = await _currencyService.GetUserCurrenciesAsync(userId);
        return Ok(userCurrencies);
    }

    /// <summary>
    /// Получение списка всех доступных валют.
    /// </summary>
    /// <returns>Список всех валют.</returns>
    [HttpGet("all-currencies")]
    [SwaggerOperation(Summary = "Получение списка всех доступных валют")]
    public async Task<IActionResult> GetAllCurrencies()
    {
        var currencies = await _currencyService.GetAllCurrenciesAsync();
        return Ok(currencies);
    }
}