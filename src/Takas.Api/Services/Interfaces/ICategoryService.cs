using Takas.Api.DTOs.Categories;

namespace Takas.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request, CancellationToken cancellationToken = default);
}
