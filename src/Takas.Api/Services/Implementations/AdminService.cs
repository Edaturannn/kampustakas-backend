using Takas.Api.Data;
using Takas.Api.DTOs.Admin;
using Takas.Api.DTOs.Notifications;
using Takas.Api.DTOs.Offers;
using Takas.Api.DTOs.Products;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class AdminService(AppDbContext dbContext) : IAdminService
{
    public async Task<AdminDashboardResponseDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        return new AdminDashboardResponseDto
        {
            TotalUsers = await dbContext.Users.CountAsync(cancellationToken),
            TotalProducts = await dbContext.Products.CountAsync(cancellationToken),
            TotalOffers = await dbContext.Offers.CountAsync(cancellationToken),
            PendingReports = await dbContext.Reports.CountAsync(x => x.Status == Enums.ReportStatus.Pending, cancellationToken),
            TotalNotifications = await dbContext.Notifications.CountAsync(cancellationToken)
        };
    }

    public async Task<List<AdminUserResponseDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await dbContext.Users
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return users.Select(x => x.ToAdminUserResponse()).ToList();
    }

    public async Task<AdminUserResponseDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                   ?? throw new NotFoundException("Kullanıcı bulunamadı.");

        return user.ToAdminUserResponse();
    }

    public async Task<List<ProductResponseDto>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await dbContext.Products
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .Include(x => x.ProductImages)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return products.Select(x => x.ToProductResponse()).ToList();
    }

    public async Task<List<OfferResponseDto>> GetOffersAsync(CancellationToken cancellationToken = default)
    {
        var offers = await dbContext.Offers
            .Include(x => x.Product)
                .ThenInclude(x => x.Owner)
            .Include(x => x.Sender)
            .Include(x => x.OfferedProduct)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return offers.Select(x => x.ToOfferResponse()).ToList();
    }

    public async Task<List<NotificationResponseDto>> GetNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var notifications = await dbContext.Notifications
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications.Select(x => x.ToNotificationResponse()).ToList();
    }
}
