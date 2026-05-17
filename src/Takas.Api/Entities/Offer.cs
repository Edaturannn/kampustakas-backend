using Takas.Api.Enums;

namespace Takas.Api.Entities;

public class Offer
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int SenderId { get; set; }
    public int? OfferedProductId { get; set; }
    public decimal? CashAmount { get; set; }
    public string? Message { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public User Sender { get; set; } = null!;
    public Product? OfferedProduct { get; set; }
    public DeliveryVerification? DeliveryVerification { get; set; }
}
