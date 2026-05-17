using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.Notifications;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class NotificationsController(INotificationService notificationService) : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<NotificationResponseDto>>>> GetNotifications(CancellationToken cancellationToken)
    {
        var response = await notificationService.GetNotificationsAsync(cancellationToken);
        return Success(response, "Bildirimler getirildi.");
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<NotificationUnreadCountResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NotificationUnreadCountResponseDto>>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var response = await notificationService.GetUnreadCountAsync(cancellationToken);
        return Success(response, "Okunmamış bildirim sayısı getirildi.");
    }

    [HttpPost("{id:int}/read")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NotificationResponseDto>>> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var response = await notificationService.MarkAsReadAsync(id, cancellationToken);
        return Success(response, "Bildirim okundu olarak işaretlendi.");
    }
}
