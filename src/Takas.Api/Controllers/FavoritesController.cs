using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.Favorites;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class FavoritesController(IFavoriteService favoriteService) : BaseApiController
{
    [HttpPost("{productId:int}")]
    [ProducesResponseType(typeof(ApiResponse<FavoriteResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<FavoriteResponseDto>>> AddFavorite(int productId, CancellationToken cancellationToken)
    {
        var response = await favoriteService.AddFavoriteAsync(productId, cancellationToken);
        return Success(response, "Ürün favorilere eklendi.", StatusCodes.Status201Created);
    }

    [HttpDelete("{productId:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<object?>>> RemoveFavorite(int productId, CancellationToken cancellationToken)
    {
        await favoriteService.RemoveFavoriteAsync(productId, cancellationToken);
        return SuccessMessage("Ürün favorilerden çıkarıldı.");
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<FavoriteResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<FavoriteResponseDto>>>> GetFavorites(CancellationToken cancellationToken)
    {
        var response = await favoriteService.GetFavoritesAsync(cancellationToken);
        return Success(response, "Favoriler getirildi.");
    }
}
