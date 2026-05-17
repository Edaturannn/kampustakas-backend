using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.Products;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Route("api/[controller]")]
public class ProductsController(IProductService productService) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetProducts(CancellationToken cancellationToken)
    {
        var response = await productService.GetProductsAsync(cancellationToken);
        return Success(response, "Ürünler getirildi.");
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetProductById(int id, CancellationToken cancellationToken)
    {
        var response = await productService.GetProductByIdAsync(id, cancellationToken);
        return Success(response, "Ürün detayı getirildi.");
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> CreateProduct(
        [FromBody] CreateProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await productService.CreateProductAsync(request, cancellationToken);
        return Success(response, "Ürün oluşturuldu.", StatusCodes.Status201Created);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> UpdateProduct(
        int id,
        [FromBody] UpdateProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await productService.UpdateProductAsync(id, request, cancellationToken);
        return Success(response, "Ürün güncellendi.");
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        await productService.DeleteProductAsync(id, cancellationToken);
        return SuccessMessage("Ürün silindi.");
    }

    [Authorize]
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetMyProducts(CancellationToken cancellationToken)
    {
        var response = await productService.GetMyProductsAsync(cancellationToken);
        return Success(response, "Kullanıcının ürünleri getirildi.");
    }

    [AllowAnonymous]
    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetProductsByUserId(int userId, CancellationToken cancellationToken)
    {
        var response = await productService.GetProductsByUserIdAsync(userId, cancellationToken);
        return Success(response, "Kullanıcıya ait ürünler getirildi.");
    }
}
