using Takas.Api.Enums;

namespace Takas.Api.DTOs.Reports;

public class ReportResponseDto
{
    public int Id { get; set; }
    public int ReporterId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public int? ReportedUserId { get; set; }
    public string? ReportedUserName { get; set; }
    public int? ProductId { get; set; }
    public string? ProductTitle { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
