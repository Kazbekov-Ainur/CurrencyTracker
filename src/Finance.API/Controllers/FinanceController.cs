using CurrencyTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    [HttpGet("user-currencies")]
    public async Task<IActionResult> GetUserCurrencies()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var userCurrencies = await _currencyService.GetUserCurrenciesAsync(userId);
        return Ok(userCurrencies);
    }

    [HttpGet("all-currencies")]
    public async Task<IActionResult> GetAllCurrencies()
    {
        var currencies = await _currencyService.GetAllCurrenciesAsync();
        return Ok(currencies);
    }
}