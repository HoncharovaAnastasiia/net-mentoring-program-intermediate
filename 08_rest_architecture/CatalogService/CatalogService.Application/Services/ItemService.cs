using AutoMapper;
using CatalogService.Application.DTOs.Common;
using CatalogService.Application.DTOs.Item;
using CatalogService.Application.Interfaces;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Exceptions;
using CatalogService.Domain.Interfaces;

namespace CatalogService.Application.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public ItemService(IItemRepository itemRepository, ICategoryRepository categoryRepository, IMapper mapper)
    {
        _itemRepository = itemRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ItemResponse>> GetPagedAsync(int? categoryId = null, int page = 1, int pageSize = 10)
    {
        if (categoryId.HasValue && !await _categoryRepository.ExistsAsync(categoryId.Value))
        {
            throw new EntityNotFoundException(nameof(Category), categoryId.Value);
        }

        var (items, totalCount) = await _itemRepository.GetPagedAsync(categoryId, page, pageSize);
        var itemResponses = _mapper.Map<IEnumerable<ItemResponse>>(items);

        return new PagedResponse<ItemResponse>(itemResponses, page, pageSize, totalCount);
    }

    public async Task<ItemResponse> GetByIdAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Item), id);

        return _mapper.Map<ItemResponse>(item);
    }

    public async Task<ItemResponse> CreateAsync(SaveItemRequest request)
    {
        if (!await _categoryRepository.ExistsAsync(request.CategoryId))
        {
            throw new EntityNotFoundException(nameof(Category), request.CategoryId);
        }

        var item = _mapper.Map<Item>(request);
        var created = await _itemRepository.AddAsync(item);

        var reloaded = await _itemRepository.GetByIdAsync(created.Id);
        return _mapper.Map<ItemResponse>(reloaded);
    }

    public async Task<ItemResponse> UpdateAsync(int id, SaveItemRequest request)
    {
        var item = await _itemRepository.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Item), id);

        if (!await _categoryRepository.ExistsAsync(request.CategoryId))
        {
            throw new EntityNotFoundException(nameof(Category), request.CategoryId);
        }

        _mapper.Map(request, item);
        var updated = await _itemRepository.UpdateAsync(item);

        return _mapper.Map<ItemResponse>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id) ?? throw new EntityNotFoundException(nameof(Item), id);

        await _itemRepository.DeleteAsync(item);
    }
}