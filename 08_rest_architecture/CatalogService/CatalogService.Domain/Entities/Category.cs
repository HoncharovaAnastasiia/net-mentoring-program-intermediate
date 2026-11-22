using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities;

public class Category : BaseEntity
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}