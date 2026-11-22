using CatalogService.API.Middleware;

namespace CatalogService.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}