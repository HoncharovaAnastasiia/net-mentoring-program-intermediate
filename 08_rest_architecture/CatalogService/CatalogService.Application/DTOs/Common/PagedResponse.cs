namespace CatalogService.Application.DTOs.Common;

public class PagedResponse<T>
{
    public PagedResponse() { }

    public PagedResponse(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public int TotalCount { get; set; }
    
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    public bool HasPreviousPage => Page > 1;
    
    public bool HasNextPage => Page < TotalPages;
}