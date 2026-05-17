using Takas.Api.Enums;

namespace Takas.Api.DTOs.Offers;

public class OfferResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public int ProductOwnerId { get; set; }
    public string ProductOwnerName { get; set; } = string.Empty;
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public int? OfferedProductId { get; set; }
    public string? OfferedProductTitle { get; set; }
    public decimal? CashAmount { get; set; }
    public string? Message { get; set; }
    public OfferStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
