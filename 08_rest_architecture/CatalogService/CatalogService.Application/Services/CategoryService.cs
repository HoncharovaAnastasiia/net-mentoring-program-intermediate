using AutoMapper;
using CatalogService.Application.DTOs.Category;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Exceptions;
using CatalogService.Domain.Interfaces;

namespace CatalogService.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Category), id);

        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> CreateAsync(SaveCategoryRequest request)
    {
        var created = await _categoryRepository.AddAsync(_mapper.Map<Category>(request));

        return _mapper.Map<CategoryResponse>(created);
    }

    public async Task<CategoryResponse> UpdateAsync(int id, SaveCategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Category), id);

        _mapper.Map(request, category);
        var updated = await _categoryRepository.UpdateAsync(category);

        return _mapper.Map<CategoryResponse>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Category), id);

        await _categoryRepository.DeleteAsync(category);
    }
}