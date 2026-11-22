using CatalogService.API.Extensions;
using CatalogService.Application.Extensions;
using CatalogService.Infrastructure.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

await app.Services.SeedDatabaseAsync();

app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();