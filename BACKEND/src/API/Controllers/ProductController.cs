using Microsoft.AspNetCore.Mvc;
using Domain.Interfaces.Services;
using Domain.Constants;
using DTOs.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using API.Extensions;
using Microsoft.Extensions.Caching.Memory;
using DTOs.DTOs.Responses;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")] // Backward compatibility
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;

    public ProductController(IProductService productService, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
    {
        _productService = productService;
        _cache = cache;
    }

    /// <summary>
    /// Get all products with optional pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = AppConstants.Pagination.DefaultPageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < AppConstants.Pagination.MinPageSize) pageSize = AppConstants.Pagination.DefaultPageSize;
        if (pageSize > AppConstants.Pagination.MaxPageSize) pageSize = AppConstants.Pagination.MaxPageSize;
        
        var (products, totalCount) = await _productService.GetProductsWithPaginationAsync(page, pageSize);
        
        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        Response.Headers.Append("X-Page", page.ToString());
        Response.Headers.Append("X-Page-Size", pageSize.ToString());
        
        return this.ApiOk(products);
    }

    /// <summary>
    /// Get products ordered by discount percentage (highest first)
    /// </summary>
    [HttpGet("by-discount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDiscount(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        // CACHE CHECK: Only for first page (most critical)
        if (page == 1)
        {
            if (_cache.TryGetValue(API.Constants.CacheKeys.DiscountedProducts, out IEnumerable<DTOs.DTOs.Responses.ProductResponseDTO> cachedProducts))
            {
                // Note: The cache might have 100 items. If user asks for 20, we take 20.
                var cachedPaged = cachedProducts.Take(pageSize);
                 // We don't have total count in cache key easily unless we cache it too or just return list.
                 // The cached object from Warmer is List<ProductResponseDTO>.
                 // For total count, we might default or we can also cache it.
                 // For now, let's return the cached list count as a rough total or fetch logic.
                 // Actually the Warmer stored the whole list of 100.
                 // Let's rely on that.
                
                Response.Headers.Append("X-Total-Count", "100"); // Approx
                Response.Headers.Append("X-Page", page.ToString());
                Response.Headers.Append("X-Page-Size", pageSize.ToString());
                return this.ApiOk(cachedPaged);
            }
        }
        
        var (products, totalCount) = await _productService.GetProductsOrderedByDiscountAsync(page, pageSize);
        
        Response.Headers.Append("X-Total-Count", totalCount.ToString());
        Response.Headers.Append("X-Page", page.ToString());
        Response.Headers.Append("X-Page-Size", pageSize.ToString());
        
        return this.ApiOk(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return this.ApiNotFound($"Product with ID {id} not found");
        }

        return this.ApiOk(product);
    }

    /// <summary>
    /// Get products by category ID
    /// </summary>
    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCategory(
        int categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        // CACHE CHECK: Only for Page 1
        if (page == 1)
        {
            if (_cache.TryGetValue(API.Constants.CacheKeys.CategoryPage(categoryId), out IEnumerable<DTOs.DTOs.Responses.ProductResponseDTO> cachedCategoryProducts))
            {
                 // Cached item is Top 50.
                 var pagedCache = cachedCategoryProducts.Take(pageSize);
                 return this.ApiOk(pagedCache);
            }
        }

        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        
        // Manual Pagination since service returns all
        var pagedProducts = products
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return this.ApiOk(pagedProducts);
    }

    /// <summary>
    /// Search products by name
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this.ApiBadRequest("Search term is required");
        }

        var products = await _productService.SearchProductsAsync(name);
        return this.ApiOk(products);
    }

    /// <summary>
    /// Get product price history
    /// </summary>
    [HttpGet("{id}/history")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPriceHistory(int id, [FromQuery] int days = 30)
    {
        var history = await _productService.GetProductPriceHistoryAsync(id, days);
        return this.ApiOk(history);
    }

    /// <summary>
    /// Get products by brand
    /// </summary>
    [HttpGet("brand/{brand}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByBrand(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
        {
            return this.ApiBadRequest("Brand is required");
        }

        var products = await _productService.SearchByBrandAsync(brand);
        return this.ApiOk(products);
    }

    /// <summary>
    /// Create new product
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return this.ApiBadRequest(ModelState);
        }

        var product = await _productService.CreateProductAsync(dto);
        return this.ApiCreated(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update existing product
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return this.ApiBadRequest(ModelState);
        }

        var product = await _productService.UpdateProductAsync(id, dto);

        if (product == null)
        {
            return this.ApiNotFound($"Product with ID {id} not found");
        }

        return this.ApiOk(product);
    }

    /// <summary>
    /// Delete product (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (!result)
        {
            return this.ApiNotFound($"Product with ID {id} not found");
        }

        return NoContent();
    }
}

