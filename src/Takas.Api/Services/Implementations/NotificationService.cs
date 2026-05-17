using Takas.Api.Data;
using Takas.Api.DTOs.Notifications;
using Takas.Api.Entities;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class NotificationService(
    AppDbContext dbContext,
    ICurrentUserAccessor currentUserAccessor) : INotificationService
{
    public async Task<List<NotificationResponseDto>> GetNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var notifications = await dbContext.Notifications
            .Where(x => x.UserId == currentUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return notifications.Select(x => x.ToNotificationResponse()).ToList();
    }

    public async Task<NotificationUnreadCountResponseDto> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var count = await dbContext.Notifications.CountAsync(
            x => x.UserId == currentUserId && !x.IsRead,
            cancellationToken);

        return new NotificationUnreadCountResponseDto
        {
            Count = count
        };
    }

    public async Task<NotificationResponseDto> MarkAsReadAsync(int id, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserId, cancellationToken)
            ?? throw new NotFoundException("Bildirim bulunamadı.");

        notification.IsRead = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return notification.ToNotificationResponse();
    }

    public async Task CreateAsync(int userId, string title, string message, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title.Trim(),
            Message = message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
