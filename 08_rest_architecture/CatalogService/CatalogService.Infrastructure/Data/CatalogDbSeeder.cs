using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Data;

public static class CatalogDbSeeder
{
    public static async Task SeedAsync(CatalogDbContext context)
    {
        if (await context.Categories.AnyAsync()) return;

        var categories = GetCategories();
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var items = GetItems();
        await context.Items.AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    private static List<Category> GetCategories() =>
    [
        new() { Name = "Electronics", Description = "Electronic devices and gadgets" },
        new() { Name = "Books", Description = "Physical and digital books" },
        new() { Name = "Clothing", Description = "Apparel and accessories" }
    ];

    private static List<Item> GetItems() =>
    [
        new() { Name = "Smartphone", Description = "Latest model smartphone", Price = 999.99m, CategoryId = 1 },
        new() { Name = "Laptop", Description = "High-performance laptop", Price = 1499.99m, CategoryId = 1 },
        new() { Name = "Headphones", Description = "Wireless noise-canceling headphones", Price = 299.99m, CategoryId = 1 },
        new() { Name = "C# in Depth", Description = "Advanced C# programming book", Price = 49.99m, CategoryId = 2 },
        new() { Name = "Clean Code", Description = "Software craftsmanship guide", Price = 39.99m, CategoryId = 2 },
        new() { Name = "T-Shirt", Description = "Cotton casual t-shirt", Price = 29.99m, CategoryId = 3 },
        new() { Name = "Jeans", Description = "Classic denim jeans", Price = 79.99m, CategoryId = 3 }
    ];
}