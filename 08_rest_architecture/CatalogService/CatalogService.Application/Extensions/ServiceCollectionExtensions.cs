using CatalogService.Application.Interfaces;
using CatalogService.Application.Mappings;
using CatalogService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IItemService, ItemService>();

        return services;
    }
}