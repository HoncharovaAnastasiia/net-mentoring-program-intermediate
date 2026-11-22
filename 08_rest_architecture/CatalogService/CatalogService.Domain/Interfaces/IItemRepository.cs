using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    Task<(IEnumerable<Item> Items, int TotalCount)> GetPagedAsync(int? categoryId = null, int page = 1, int pageSize = 10);
}