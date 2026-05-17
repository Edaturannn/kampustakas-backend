using Takas.Api.Data;
using Takas.Api.DTOs.Favorites;
using Takas.Api.Entities;
using Takas.Api.Enums;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class FavoriteService(
    AppDbContext dbContext,
    ICurrentUserAccessor currentUserAccessor) : IFavoriteService
{
    public async Task<List<FavoriteResponseDto>> GetFavoritesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var favorites = await BuildFavoriteQuery()
            .Where(x => x.UserId == currentUserId && x.Product.Status != ProductStatus.Deleted)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        return favorites.Select(x => x.ToFavoriteResponse()).ToList();
    }

    public async Task<FavoriteResponseDto> AddFavoriteAsync(int productId, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var productExists = await dbContext.Products.AnyAsync(
            x => x.Id == productId && x.Status != ProductStatus.Deleted,
            cancellationToken);

        if (!productExists)
        {
            throw new NotFoundException("Favorilere eklenecek ürün bulunamadı.");
        }

        var favoriteExists = await dbContext.Favorites.AnyAsync(
            x => x.UserId == currentUserId && x.ProductId == productId,
            cancellationToken);

        if (favoriteExists)
        {
            throw new ConflictException("Bu ürün zaten favorilerinizde.");
        }

        var favorite = new Favorite
        {
            UserId = currentUserId,
            ProductId = productId
        };

        dbContext.Favorites.Add(favorite);
        await dbContext.SaveChangesAsync(cancellationToken);

        var savedFavorite = await BuildFavoriteQuery()
            .FirstAsync(x => x.Id == favorite.Id, cancellationToken);

        return savedFavorite.ToFavoriteResponse();
    }

    public async Task RemoveFavoriteAsync(int productId, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var favorite = await dbContext.Favorites.FirstOrDefaultAsync(
            x => x.UserId == currentUserId && x.ProductId == productId,
            cancellationToken)
            ?? throw new NotFoundException("Favori kaydı bulunamadı.");

        dbContext.Favorites.Remove(favorite);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Favorite> BuildFavoriteQuery()
    {
        return dbContext.Favorites
            .Include(x => x.Product)
                .ThenInclude(x => x.Owner)
            .Include(x => x.Product)
                .ThenInclude(x => x.Category)
            .Include(x => x.Product)
                .ThenInclude(x => x.ProductImages);
    }
}
