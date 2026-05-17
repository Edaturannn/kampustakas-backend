using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.Offers;

public class CreateOfferRequestDto
{
    [Required(ErrorMessage = "Teklif yapılacak ürün zorunludur.")]
    public int ProductId { get; set; }

    public int? OfferedProductId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Nakit teklif sıfırdan küçük olamaz.")]
    public decimal? CashAmount { get; set; }

    [MaxLength(1000, ErrorMessage = "Teklif mesajı en fazla 1000 karakter olabilir.")]
    public string? Message { get; set; }
}
