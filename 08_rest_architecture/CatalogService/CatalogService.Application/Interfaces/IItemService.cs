using CatalogService.Application.DTOs.Common;
using CatalogService.Application.DTOs.Item;

namespace CatalogService.Application.Interfaces;

public interface IItemService
{
    Task<PagedResponse<ItemResponse>> GetPagedAsync(int? categoryId = null, int page = 1, int pageSize = 10);

    Task<ItemResponse> GetByIdAsync(int id);
    
    Task<ItemResponse> CreateAsync(SaveItemRequest request);
    
    Task<ItemResponse> UpdateAsync(int id, SaveItemRequest request);
    
    Task DeleteAsync(int id);
}