using Takas.Api.DTOs.Notifications;

namespace Takas.Api.Services.Interfaces;

public interface INotificationService
{
    Task<List<NotificationResponseDto>> GetNotificationsAsync(CancellationToken cancellationToken = default);
    Task<NotificationUnreadCountResponseDto> GetUnreadCountAsync(CancellationToken cancellationToken = default);
    Task<NotificationResponseDto> MarkAsReadAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(int userId, string title, string message, CancellationToken cancellationToken = default);
}
