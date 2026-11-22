using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(CatalogDbContext context) : base(context) { }

    public async Task<(IEnumerable<Item> Items, int TotalCount)> GetPagedAsync(int? categoryId = null, int page = 1, int pageSize = 10)
    {
        var query = _dbSet.AsQueryable();

        if (categoryId.HasValue) query = query.Where(item => item.CategoryId == categoryId.Value);

        var totalCount = await query.CountAsync();
        var items = await query.Include(item => item.Category).OrderBy(item => item.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public override async Task<Item?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(item => item.Category).FirstOrDefaultAsync(item => item.Id == id);
    }

    public override async Task<IEnumerable<Item>> GetAllAsync()
    {
        return await _dbSet.Include(item => item.Category).OrderBy(item => item.Name).ToListAsync();
    }
}