using Takas.Api.DTOs.ContactMessages;

namespace Takas.Api.Services.Interfaces;

public interface IContactMessageService
{
    Task<ContactMessageResponseDto> CreateMessageAsync(CreateContactMessageRequestDto request, CancellationToken cancellationToken = default);
    Task<List<ContactMessageResponseDto>> GetMyMessagesAsync(CancellationToken cancellationToken = default);
    Task<List<ContactMessageResponseDto>> GetAdminMessagesAsync(CancellationToken cancellationToken = default);
    Task<ContactMessageResponseDto> GetAdminMessageByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ContactMessageResponseDto> ReplyToMessageAsync(int id, ReplyContactMessageRequestDto request, CancellationToken cancellationToken = default);
    Task<ContactMessageResponseDto> UpdateStatusAsync(int id, UpdateContactMessageStatusRequestDto request, CancellationToken cancellationToken = default);
}
