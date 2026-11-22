using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string databaseName = "CatalogDb")
    {
        services.AddDbContext<CatalogDbContext>(options => options.UseInMemoryDatabase(databaseName));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();

        return services;
    }

    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        await CatalogDbSeeder.SeedAsync(context);
    }
}