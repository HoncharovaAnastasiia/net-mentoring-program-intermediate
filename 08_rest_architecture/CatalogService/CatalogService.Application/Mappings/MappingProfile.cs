using AutoMapper;
using CatalogService.Application.DTOs.Category;
using CatalogService.Application.DTOs.Item;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryResponse>();
        CreateMap<SaveCategoryRequest, Category>();

        CreateMap<SaveItemRequest, Item>();
        CreateMap<Item, ItemResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));
    }
}