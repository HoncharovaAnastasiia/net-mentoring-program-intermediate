using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCategoryEntity(modelBuilder);
        ConfigureItemEntity(modelBuilder);
    }

    private static void ConfigureCategoryEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasMany(category => category.Items)
                  .WithOne(item => item.Category)
                  .HasForeignKey(item => item.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(category => category.Name);
        });
    }

    private static void ConfigureItemEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasIndex(item => item.CategoryId);
            entity.HasIndex(item => new { item.CategoryId, item.Name });
        });
    }
}