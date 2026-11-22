using CatalogService.API.Models;
using CatalogService.Application.DTOs.Common;
using CatalogService.Application.DTOs.Item;
using CatalogService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    /// <summary>
    /// Gets paginated list of items with optional category filter
    /// </summary>
    /// <param name="categoryId">Optional category ID filter</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of items</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResponse<ItemResponse>>> GetAll(
        [FromQuery] int? categoryId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var items = await _itemService.GetPagedAsync(categoryId, page, pageSize);
        return Ok(items);
    }

    /// <summary>
    /// Gets an item by ID
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <returns>Item details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> GetById(int id)
    {
        var item = await _itemService.GetByIdAsync(id);
        return Ok(item);
    }

    /// <summary>
    /// Creates a new item
    /// </summary>
    /// <param name="request">Item data</param>
    /// <returns>Created item</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> Create([FromBody] SaveItemRequest request)
    {
        var item = await _itemService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    /// <summary>
    /// Updates an existing item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="request">Updated item data</param>
    /// <returns>Updated item</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemResponse>> Update(int id, [FromBody] SaveItemRequest request)
    {
        var item = await _itemService.UpdateAsync(id, request);
        return Ok(item);
    }

    /// <summary>
    /// Deletes an item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _itemService.DeleteAsync(id);
        return NoContent();
    }
}