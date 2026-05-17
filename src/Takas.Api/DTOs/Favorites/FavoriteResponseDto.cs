using Takas.Api.Enums;

namespace Takas.Api.DTOs.Favorites;

public class FavoriteResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }
    public ProductStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
