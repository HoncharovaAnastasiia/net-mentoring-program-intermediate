using CatalogService.API.Models;
using CatalogService.Application.DTOs.Category;
using CatalogService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <returns>List of categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Gets a category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>Category details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(category);
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="request">Category data</param>
    /// <returns>Created category</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] SaveCategoryRequest request)
    {
        var category = await _categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Updated category data</param>
    /// <returns>Updated category</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> Update(int id, [FromBody] SaveCategoryRequest request)
    {
        var category = await _categoryService.UpdateAsync(id, request);
        return Ok(category);
    }

    /// <summary>
    /// Deletes a category and all related items
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}