using Takas.Api.Data;
using Takas.Api.DTOs.Categories;
using Takas.Api.Entities;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class CategoryService(AppDbContext dbContext) : ICategoryService
{
    public async Task<List<CategoryResponseDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await dbContext.Categories
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(x => x.ToCategoryResponse()).ToList();
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryRequestDto request, CancellationToken cancellationToken = default)
    {
        var categoryName = request.Name.Trim();

        var exists = await dbContext.Categories.AnyAsync(
            x => x.Name.ToLower() == categoryName.ToLower(),
            cancellationToken);

        if (exists)
        {
            throw new ConflictException("Bu kategori zaten mevcut.");
        }

        var category = new Category
        {
            Name = categoryName
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return category.ToCategoryResponse();
    }
}
