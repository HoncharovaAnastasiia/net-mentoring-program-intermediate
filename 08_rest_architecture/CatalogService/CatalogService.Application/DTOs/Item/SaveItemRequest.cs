using System.ComponentModel.DataAnnotations;

namespace CatalogService.Application.DTOs.Item;

public class SaveItemRequest
{
    [Required(ErrorMessage = "Item name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Category ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Category ID must be a positive integer")]
    public int CategoryId { get; set; }
}