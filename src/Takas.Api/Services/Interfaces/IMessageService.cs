using Takas.Api.DTOs.Messages;

namespace Takas.Api.Services.Interfaces;

public interface IMessageService
{
    Task<MessageResponseDto> SendMessageAsync(SendMessageRequestDto request, CancellationToken cancellationToken = default);
    Task<List<MessageResponseDto>> GetConversationAsync(int userId, CancellationToken cancellationToken = default);
    Task<UnreadCountResponseDto> GetUnreadCountAsync(CancellationToken cancellationToken = default);
}
