namespace Takas.Api.Entities;

public class DeliveryVerification
{
    public int Id { get; set; }
    public int OfferId { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Offer Offer { get; set; } = null!;
}
