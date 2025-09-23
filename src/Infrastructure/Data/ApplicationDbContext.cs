using CurrencyTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CurrencyTracker.Infrastructure.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Currency> Currencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связи многие-ко-многим между User и Currency
        modelBuilder.Entity<User>()
            .HasMany(u => u.FavoriteCurrencies)
            .WithMany(c => c.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserCurrencyFavorites",
                j => j.HasOne<Currency>().WithMany().HasForeignKey("CurrencyId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j =>
                {
                    j.HasKey("UserId", "CurrencyId");
                    j.HasIndex("CurrencyId");
                });

        // Уникальные constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Currency>()
            .HasIndex(c => c.Code)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}