using CrudPractice.Domain.Interfaces;
using CrudPractice.Domain.Interfaces.Repositories;
using CrudPractice.Infrastructure.Persistence;
using CrudPractice.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace CrudPractice.Infrastructure;

/// <summary>
/// [Design Pattern - Extension Method / Facade]
/// Infrastructure DI registration.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ─── Database (EF Core + PostgreSQL) ─────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql =>
                {
                    npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    npgsql.CommandTimeout(60);
                })
            .UseSnakeCaseNamingConvention()
            .EnableDetailedErrors(false)
            .EnableSensitiveDataLogging(false)
        );

        // ─── Dapper: NpgsqlDataSource cho raw SQL ─────────────────────────
        services.AddSingleton<NpgsqlDataSource>(_ =>
            NpgsqlDataSource.Create(configuration.GetConnectionString("DefaultConnection")!));

        // ─── Repositories ────────────────────────────────────────────────
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
