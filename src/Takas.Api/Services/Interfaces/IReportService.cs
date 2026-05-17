using Takas.Api.DTOs.Reports;

namespace Takas.Api.Services.Interfaces;

public interface IReportService
{
    Task<ReportResponseDto> CreateReportAsync(CreateReportRequestDto request, CancellationToken cancellationToken = default);
    Task<List<ReportResponseDto>> GetAdminReportsAsync(CancellationToken cancellationToken = default);
}
