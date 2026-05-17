using Takas.Api.Data;
using Takas.Api.DTOs.Reports;
using Takas.Api.Entities;
using Takas.Api.Enums;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class ReportService(
    AppDbContext dbContext,
    ICurrentUserAccessor currentUserAccessor) : IReportService
{
    public async Task<ReportResponseDto> CreateReportAsync(CreateReportRequestDto request, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        if (!request.ReportedUserId.HasValue && !request.ProductId.HasValue)
        {
            throw new BadRequestException("Şikayet için kullanıcı veya ürün bilgisi vermelisiniz.");
        }

        Product? product = null;
        if (request.ProductId.HasValue)
        {
            product = await dbContext.Products
                .Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == request.ProductId.Value && x.Status != ProductStatus.Deleted, cancellationToken)
                ?? throw new NotFoundException("Şikayet edilen ürün bulunamadı.");
        }

        User? reportedUser = null;
        var reportedUserId = request.ReportedUserId ?? product?.OwnerId;

        if (reportedUserId.HasValue)
        {
            reportedUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == reportedUserId.Value, cancellationToken)
                           ?? throw new NotFoundException("Şikayet edilen kullanıcı bulunamadı.");
        }

        var report = new Report
        {
            ReporterId = currentUserId,
            ReportedUserId = reportedUserId,
            ProductId = product?.Id,
            Reason = request.Reason.Trim(),
            Status = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Reports.Add(report);
        await dbContext.SaveChangesAsync(cancellationToken);

        var savedReport = await dbContext.Reports
            .Include(x => x.Reporter)
            .Include(x => x.ReportedUser)
            .Include(x => x.Product)
            .FirstAsync(x => x.Id == report.Id, cancellationToken);

        return savedReport.ToReportResponse();
    }

    public async Task<List<ReportResponseDto>> GetAdminReportsAsync(CancellationToken cancellationToken = default)
    {
        var reports = await dbContext.Reports
            .Include(x => x.Reporter)
            .Include(x => x.ReportedUser)
            .Include(x => x.Product)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return reports.Select(x => x.ToReportResponse()).ToList();
    }
}
