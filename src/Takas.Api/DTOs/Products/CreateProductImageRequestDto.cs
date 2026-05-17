using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.Products;

public class CreateProductImageRequestDto
{
    [Required(ErrorMessage = "Ürün görseli zorunludur.")]
    [Url(ErrorMessage = "Görsel adresi geçerli bir URL olmalıdır.")]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsMain { get; set; }
}
