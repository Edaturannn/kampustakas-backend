using System.ComponentModel.DataAnnotations;

namespace Takas.Api.DTOs.Reports;

public class CreateReportRequestDto
{
    public int? ReportedUserId { get; set; }
    public int? ProductId { get; set; }

    [Required(ErrorMessage = "Şikayet nedeni zorunludur.")]
    [MaxLength(1000, ErrorMessage = "Şikayet nedeni en fazla 1000 karakter olabilir.")]
    public string Reason { get; set; } = string.Empty;
}
