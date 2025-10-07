using CurrencyTracker.Application.Interfaces;
using CurrencyTracker.Application.Services;
using CurrencyTracker.Finance.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace CurrencyTracker.Finance.Grpc.Services;

[Authorize] 
public class CurrencyGrpcService : CurrencyService.CurrencyServiceBase
{
    private readonly ICurrencyService _currencyService;

    public CurrencyGrpcService(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    public override async Task<GetUserCurrenciesResponse> GetUserCurrencies(
        GetUserCurrenciesRequest request, ServerCallContext context)
    {

        var userId = request.UserId;

        var result = await _currencyService.GetUserCurrenciesAsync(userId);

        var response = new GetUserCurrenciesResponse();

        if (result?.User == null)
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        // Маппинг User
        response.User = new GetUserCurrenciesResponse.Types.User
        {
            Id = result.User.Id,
            Name = result.User.Name,
            Email = result.User.Email,
            CreatedAt = ((DateTimeOffset)result.User.CreatedAt).ToUnixTimeSeconds()
        };

        // Маппинг Currencies
        foreach (var currency in result.FavoriteCurrencies)
        {
            response.Currencies.Add(new GetUserCurrenciesResponse.Types.Currency
            {
                Id = currency.Id,
                Name = currency.Name,
                Code = currency.Code,
                Rate = (double)currency.Rate,
                LastUpdated = ((DateTimeOffset)currency.LastUpdated).ToUnixTimeSeconds()
            });
        }

        return response;
    }
}