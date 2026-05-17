using Takas.Api.Data;
using Takas.Api.DTOs.Products;
using Takas.Api.Entities;
using Takas.Api.Enums;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class ProductService(
    AppDbContext dbContext,
    ICurrentUserAccessor currentUserAccessor) : IProductService
{
    public async Task<List<ProductResponseDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await BuildProductQuery()
            .Where(x => x.Status != ProductStatus.Deleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return await MapProductsAsync(products, cancellationToken);
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id && x.Status != ProductStatus.Deleted, cancellationToken)
            ?? throw new NotFoundException("Ürün bulunamadı.");

        product.ViewCount += 1;
        await dbContext.SaveChangesAsync(cancellationToken);

        var favoriteProductIds = await GetFavoriteProductIdsAsync(new[] { product.Id }, cancellationToken);
        return product.ToProductResponse(favoriteProductIds.Contains(product.Id));
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateProductRequest(request.EstimatedMinPrice, request.EstimatedMaxPrice, request.Images);

        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();
        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == request.CategoryId, cancellationToken)
                       ?? throw new NotFoundException("Seçilen kategori bulunamadı.");

        var product = new Product
        {
            OwnerId = currentUserId,
            CategoryId = category.Id,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Condition = request.Condition.Trim(),
            EstimatedMinPrice = request.EstimatedMinPrice,
            EstimatedMaxPrice = request.EstimatedMaxPrice,
            Campus = request.Campus.Trim(),
            Status = ProductStatus.Available,
            CreatedAt = DateTime.UtcNow,
            ProductImages = NormalizeImages(request.Images)
                .Select(image => new ProductImage
                {
                    ImageUrl = image.ImageUrl.Trim(),
                    IsMain = image.IsMain
                })
                .ToList()
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(product).Reference(x => x.Owner).LoadAsync(cancellationToken);
        await dbContext.Entry(product).Reference(x => x.Category).LoadAsync(cancellationToken);

        return product.ToProductResponse();
    }

    public async Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateProductRequest(request.EstimatedMinPrice, request.EstimatedMaxPrice, request.Images);

        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var product = await dbContext.Products
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .Include(x => x.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id && x.Status != ProductStatus.Deleted, cancellationToken)
            ?? throw new NotFoundException("Ürün bulunamadı.");

        if (product.OwnerId != currentUserId)
        {
            throw new ForbiddenException("Sadece kendi ürünlerinizi güncelleyebilirsiniz.");
        }

        var category = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == request.CategoryId, cancellationToken)
                       ?? throw new NotFoundException("Seçilen kategori bulunamadı.");

        product.CategoryId = category.Id;
        product.Title = request.Title.Trim();
        product.Description = request.Description.Trim();
        product.Condition = request.Condition.Trim();
        product.EstimatedMinPrice = request.EstimatedMinPrice;
        product.EstimatedMaxPrice = request.EstimatedMaxPrice;
        product.Campus = request.Campus.Trim();

        dbContext.ProductImages.RemoveRange(product.ProductImages);
        product.ProductImages = NormalizeImages(request.Images)
            .Select(image => new ProductImage
            {
                ImageUrl = image.ImageUrl.Trim(),
                IsMain = image.IsMain
            })
            .ToList();

        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(product).Reference(x => x.Category).LoadAsync(cancellationToken);

        return product.ToProductResponse();
    }

    public async Task DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var product = await dbContext.Products.FirstOrDefaultAsync(
            x => x.Id == id && x.Status != ProductStatus.Deleted,
            cancellationToken)
            ?? throw new NotFoundException("Ürün bulunamadı.");

        if (product.OwnerId != currentUserId)
        {
            throw new ForbiddenException("Sadece kendi ürünlerinizi silebilirsiniz.");
        }

        product.Status = ProductStatus.Deleted;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProductResponseDto>> GetMyProductsAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var products = await BuildProductQuery()
            .Where(x => x.OwnerId == currentUserId && x.Status != ProductStatus.Deleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return await MapProductsAsync(products, cancellationToken);
    }

    public async Task<List<ProductResponseDto>> GetProductsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userExists = await dbContext.Users.AnyAsync(x => x.Id == userId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }

        var products = await BuildProductQuery()
            .Where(x => x.OwnerId == userId && x.Status != ProductStatus.Deleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return await MapProductsAsync(products, cancellationToken);
    }

    private IQueryable<Product> BuildProductQuery()
    {
        return dbContext.Products
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .Include(x => x.ProductImages);
    }

    private async Task<List<ProductResponseDto>> MapProductsAsync(List<Product> products, CancellationToken cancellationToken)
    {
        var favoriteProductIds = await GetFavoriteProductIdsAsync(products.Select(x => x.Id), cancellationToken);
        return products.Select(product => product.ToProductResponse(favoriteProductIds.Contains(product.Id))).ToList();
    }

    private async Task<HashSet<int>> GetFavoriteProductIdsAsync(IEnumerable<int> productIds, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserAccessor.GetCurrentUserIdOrNull();
        var ids = productIds.Distinct().ToList();

        if (!currentUserId.HasValue || ids.Count == 0)
        {
            return new HashSet<int>();
        }

        var favoriteIds = await dbContext.Favorites
            .Where(x => x.UserId == currentUserId.Value && ids.Contains(x.ProductId))
            .Select(x => x.ProductId)
            .ToListAsync(cancellationToken);

        return favoriteIds.ToHashSet();
    }

    private static void ValidateProductRequest(decimal minPrice, decimal maxPrice, IReadOnlyCollection<CreateProductImageRequestDto> images)
    {
        if (minPrice > maxPrice)
        {
            throw new BadRequestException("Minimum fiyat maksimum fiyattan büyük olamaz.");
        }

        if (images.Count(x => x.IsMain) > 1)
        {
            throw new BadRequestException("Bir üründe sadece bir ana görsel olabilir.");
        }
    }

    private static List<CreateProductImageRequestDto> NormalizeImages(IEnumerable<CreateProductImageRequestDto> images)
    {
        var normalized = images
            .Select(x => new CreateProductImageRequestDto
            {
                ImageUrl = x.ImageUrl,
                IsMain = x.IsMain
            })
            .ToList();

        if (normalized.Count > 0 && normalized.All(x => !x.IsMain))
        {
            normalized[0].IsMain = true;
        }

        return normalized;
    }
}
