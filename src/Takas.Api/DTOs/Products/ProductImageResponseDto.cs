namespace Takas.Api.DTOs.Products;

public class ProductImageResponseDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}
