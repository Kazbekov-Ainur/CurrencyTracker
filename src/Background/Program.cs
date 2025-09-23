using CurrencyTracker.Background.Services;
using CurrencyTracker.Application.Interfaces;
using CurrencyTracker.Application.Services;
using CurrencyTracker.Domain.Interfaces;
using CurrencyTracker.Infrastructure.Data;
using CurrencyTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Database
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();

        // Services
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<ITokenService, TokenService>();

        // Background Service
        services.AddHostedService<CurrencyBackgroundService>();
    })
    .Build();

await host.RunAsync();