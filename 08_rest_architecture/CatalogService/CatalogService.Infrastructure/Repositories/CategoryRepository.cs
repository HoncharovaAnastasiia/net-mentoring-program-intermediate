using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(CatalogDbContext context) : base(context) { }

    public async Task<Category?> GetByIdWithItemsAsync(int id)
    {
        return await _dbSet.Include(category => category.Items).FirstOrDefaultAsync(category => category.Id == id);
    }

    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet.OrderBy(category => category.Name).ToListAsync();
    }
}