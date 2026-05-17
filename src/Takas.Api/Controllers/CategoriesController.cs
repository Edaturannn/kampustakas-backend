using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Categories;
using Takas.Api.DTOs.Common;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Route("api/[controller]")]
public class CategoriesController(ICategoryService categoryService) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<CategoryResponseDto>>>> GetCategories(CancellationToken cancellationToken)
    {
        var response = await categoryService.GetCategoriesAsync(cancellationToken);
        return Success(response, "Kategoriler getirildi.");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> CreateCategory(
        [FromBody] CreateCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await categoryService.CreateCategoryAsync(request, cancellationToken);
        return Success(response, "Kategori oluşturuldu.", StatusCodes.Status201Created);
    }
}
