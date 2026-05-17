using Takas.Api.Enums;

namespace Takas.Api.DTOs.Products;

public class ProductResponseDto
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string? OwnerAvatarUrl { get; set; }
    public double OwnerRating { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public decimal EstimatedMinPrice { get; set; }
    public decimal EstimatedMaxPrice { get; set; }
    public string Campus { get; set; } = string.Empty;
    public ProductStatus Status { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsFavorited { get; set; }
    public List<ProductImageResponseDto> Images { get; set; } = new();
}
