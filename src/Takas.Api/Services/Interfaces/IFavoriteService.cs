using Takas.Api.DTOs.Favorites;

namespace Takas.Api.Services.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteResponseDto>> GetFavoritesAsync(CancellationToken cancellationToken = default);
    Task<FavoriteResponseDto> AddFavoriteAsync(int productId, CancellationToken cancellationToken = default);
    Task RemoveFavoriteAsync(int productId, CancellationToken cancellationToken = default);
}
