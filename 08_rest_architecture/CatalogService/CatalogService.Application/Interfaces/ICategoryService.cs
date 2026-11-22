using CatalogService.Application.DTOs.Category;

namespace CatalogService.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync();

    Task<CategoryResponse> GetByIdAsync(int id);
    
    Task<CategoryResponse> CreateAsync(SaveCategoryRequest request);
    
    Task<CategoryResponse> UpdateAsync(int id, SaveCategoryRequest request);
    
    Task DeleteAsync(int id);
}