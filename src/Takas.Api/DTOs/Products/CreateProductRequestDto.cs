using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.Products;

public class CreateProductRequestDto
{
    [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Ürün başlığı zorunludur.")]
    [MaxLength(200, ErrorMessage = "Ürün başlığı en fazla 200 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ürün açıklaması zorunludur.")]
    [MaxLength(2000, ErrorMessage = "Ürün açıklaması en fazla 2000 karakter olabilir.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ürün durumu zorunludur.")]
    [MaxLength(100, ErrorMessage = "Ürün durumu en fazla 100 karakter olabilir.")]
    public string Condition { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Minimum fiyat sıfırdan küçük olamaz.")]
    public decimal EstimatedMinPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Maksimum fiyat sıfırdan küçük olamaz.")]
    public decimal EstimatedMaxPrice { get; set; }

    [Required(ErrorMessage = "Kampüs bilgisi zorunludur.")]
    [MaxLength(100, ErrorMessage = "Kampüs bilgisi en fazla 100 karakter olabilir.")]
    public string Campus { get; set; } = string.Empty;

    public List<CreateProductImageRequestDto> Images { get; set; } = new();
}
