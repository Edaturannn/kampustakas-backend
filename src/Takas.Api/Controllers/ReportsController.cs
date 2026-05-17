using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.Reports;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ReportsController(IReportService reportService) : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReportResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ReportResponseDto>>> CreateReport(
        [FromBody] CreateReportRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await reportService.CreateReportAsync(request, cancellationToken);
        return Success(response, "Şikayet kaydı oluşturuldu.", StatusCodes.Status201Created);
    }
}
