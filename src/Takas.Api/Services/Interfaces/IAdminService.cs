using Takas.Api.DTOs.Admin;
using Takas.Api.DTOs.Notifications;
using Takas.Api.DTOs.Offers;
using Takas.Api.DTOs.Products;

namespace Takas.Api.Services.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardResponseDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<List<AdminUserResponseDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<AdminUserResponseDto> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ProductResponseDto>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<List<OfferResponseDto>> GetOffersAsync(CancellationToken cancellationToken = default);
    Task<List<NotificationResponseDto>> GetNotificationsAsync(CancellationToken cancellationToken = default);
}
