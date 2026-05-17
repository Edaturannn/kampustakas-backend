using Takas.Api.Data;
using Takas.Api.DTOs.ContactMessages;
using Takas.Api.Entities;
using Takas.Api.Enums;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class ContactMessageService(
    AppDbContext dbContext,
    ICurrentUserAccessor currentUserAccessor) : IContactMessageService
{
    public async Task<ContactMessageResponseDto> CreateMessageAsync(
        CreateContactMessageRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var currentUser = currentUserAccessor.GetRequiredCurrentUser();

        if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Message))
        {
            throw new BadRequestException("Mesaj konusu ve icerigi bos birakilamaz.");
        }

        var contactMessage = new ContactMessage
        {
            UserId = currentUser.Id,
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            Status = ContactMessageStatus.New,
            CreatedAt = DateTime.UtcNow,
            User = currentUser
        };

        dbContext.ContactMessages.Add(contactMessage);
        await dbContext.SaveChangesAsync(cancellationToken);

        return contactMessage.ToContactMessageResponse();
    }

    public async Task<List<ContactMessageResponseDto>> GetMyMessagesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var messages = await dbContext.ContactMessages
            .Include(x => x.User)
            .Where(x => x.UserId == currentUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return messages.Select(x => x.ToContactMessageResponse()).ToList();
    }

    public async Task<List<ContactMessageResponseDto>> GetAdminMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await dbContext.ContactMessages
            .Include(x => x.User)
            .OrderBy(x => x.Status)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return messages.Select(x => x.ToContactMessageResponse()).ToList();
    }

    public async Task<ContactMessageResponseDto> GetAdminMessageByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var message = await GetMessageEntityAsync(id, cancellationToken);
        return message.ToContactMessageResponse();
    }

    public async Task<ContactMessageResponseDto> ReplyToMessageAsync(
        int id,
        ReplyContactMessageRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.AdminReply))
        {
            throw new BadRequestException("Admin yaniti bos birakilamaz.");
        }

        var message = await GetMessageEntityAsync(id, cancellationToken);
        message.AdminReply = request.AdminReply.Trim();
        message.Status = ContactMessageStatus.Replied;
        message.RepliedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return message.ToContactMessageResponse();
    }

    public async Task<ContactMessageResponseDto> UpdateStatusAsync(
        int id,
        UpdateContactMessageStatusRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!request.Status.HasValue)
        {
            throw new BadRequestException("Mesaj durumu zorunludur.");
        }

        var message = await GetMessageEntityAsync(id, cancellationToken);

        if (request.Status.Value == ContactMessageStatus.Replied && string.IsNullOrWhiteSpace(message.AdminReply))
        {
            throw new BadRequestException("Admin yaniti olmadan mesaj durumu Replied yapilamaz.");
        }

        message.Status = request.Status.Value;
        await dbContext.SaveChangesAsync(cancellationToken);

        return message.ToContactMessageResponse();
    }

    private async Task<ContactMessage> GetMessageEntityAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.ContactMessages
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Iletisim mesaji bulunamadi.");
    }
}
