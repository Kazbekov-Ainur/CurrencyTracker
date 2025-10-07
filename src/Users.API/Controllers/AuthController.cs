using CurrencyTracker.Application.DTOs;
using CurrencyTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace CurrencyTracker.Users.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public AuthController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    /// <param name="registerDto">Данные для регистрации пользователя.</param>
    /// <returns>Информация о созданном пользователе.</returns>
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Регистрация нового пользователя")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
    {
        try
        {
            var user = await _userService.RegisterAsync(registerDto);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Аутентификация пользователя и получение JWT-токена.
    /// </summary>
    /// <param name="loginDto">Данные для входа (email и пароль).</param>
    /// <returns>JWT-токен для авторизации.</returns>
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Аутентификация пользователя и получение JWT-токена")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var token = await _userService.LoginAsync(loginDto);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Выход пользователя (псевдо-логаут, т.к. токен хранится на клиенте).
    /// </summary>
    /// <returns>Сообщение об успешном выходе.</returns>
    [Authorize]
    [HttpPost("logout")]
    [SwaggerOperation(Summary = "Выход пользователя из системы")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    /// Получение профиля текущего авторизованного пользователя.
    /// </summary>
    /// <returns>Информация о пользователе.</returns>
    [Authorize]
    [HttpGet("profile")]
    [SwaggerOperation(Summary = "Получение профиля текущего пользователя")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var user = await _userService.GetUserByIdAsync(userId);
        return Ok(user);
    }

    /// <summary>
    /// Добавление валюты в список избранных текущего пользователя.
    /// </summary>
    /// <param name="currencyId">Идентификатор валюты для добавления.</param>
    /// <returns>Результат операции (Ok или Unauthorized).</returns>
    [Authorize]
    [HttpPost("favorite/{currencyId}")]
    [SwaggerOperation(Summary = "Добавление валюты в избранное пользователя")]
    public async Task<IActionResult> AddFavorite(int currencyId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        await _userService.AddFavoriteCurrencyAsync(userId, currencyId);
        return Ok();
    }
}