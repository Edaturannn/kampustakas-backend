using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.Messages;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class MessagesController(IMessageService messageService) : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MessageResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MessageResponseDto>>> SendMessage(
        [FromBody] SendMessageRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await messageService.SendMessageAsync(request, cancellationToken);
        return Success(response, "Mesaj gönderildi.", StatusCodes.Status201Created);
    }

    [HttpGet("conversation/{userId:int}")]
    [ProducesResponseType(typeof(ApiResponse<List<MessageResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<MessageResponseDto>>>> GetConversation(int userId, CancellationToken cancellationToken)
    {
        var response = await messageService.GetConversationAsync(userId, cancellationToken);
        return Success(response, "Mesajlaşma geçmişi getirildi.");
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<UnreadCountResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UnreadCountResponseDto>>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var response = await messageService.GetUnreadCountAsync(cancellationToken);
        return Success(response, "Okunmamış mesaj sayısı getirildi.");
    }
}
