using Takas.Api.Enums;

namespace Takas.Api.Entities;

public class Report
{
    public int Id { get; set; }
    public int ReporterId { get; set; }
    public int? ReportedUserId { get; set; }
    public int? ProductId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Reporter { get; set; } = null!;
    public User? ReportedUser { get; set; }
    public Product? Product { get; set; }
}
