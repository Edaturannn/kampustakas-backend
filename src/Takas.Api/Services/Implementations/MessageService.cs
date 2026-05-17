using Takas.Api.Data;
using Takas.Api.DTOs.Messages;
using Takas.Api.Entities;
using Takas.Api.Enums;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class MessageService(
    AppDbContext dbContext,
    ICurrentUserAccessor currentUserAccessor,
    INotificationService notificationService) : IMessageService
{
    public async Task<MessageResponseDto> SendMessageAsync(SendMessageRequestDto request, CancellationToken cancellationToken = default)
    {
        var currentUser = currentUserAccessor.GetRequiredCurrentUser();

        if (request.ReceiverId == currentUser.Id)
        {
            throw new BadRequestException("Kendinize mesaj gönderemezsiniz.");
        }

        var receiver = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.ReceiverId, cancellationToken)
                       ?? throw new NotFoundException("Mesaj alıcısı bulunamadı.");

        if (request.ProductId.HasValue)
        {
            var productExists = await dbContext.Products.AnyAsync(
                x => x.Id == request.ProductId.Value && x.Status != ProductStatus.Deleted,
                cancellationToken);

            if (!productExists)
            {
                throw new NotFoundException("Mesajla ilişkili ürün bulunamadı.");
            }
        }

        var message = new Message
        {
            SenderId = currentUser.Id,
            ReceiverId = receiver.Id,
            ProductId = request.ProductId,
            Content = request.Content.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            Sender = currentUser,
            Receiver = receiver
        };

        dbContext.Messages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);

        await notificationService.CreateAsync(
            receiver.Id,
            "Yeni mesaj",
            $"{currentUser.FullName} size yeni bir mesaj gönderdi.",
            cancellationToken);

        return message.ToMessageResponse();
    }

    public async Task<List<MessageResponseDto>> GetConversationAsync(int userId, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var userExists = await dbContext.Users.AnyAsync(x => x.Id == userId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundException("Görüşme yapılacak kullanıcı bulunamadı.");
        }

        var unreadMessages = await dbContext.Messages
            .Where(x => x.SenderId == userId && x.ReceiverId == currentUserId && !x.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var unreadMessage in unreadMessages)
        {
            unreadMessage.IsRead = true;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var messages = await dbContext.Messages
            .Include(x => x.Sender)
            .Include(x => x.Receiver)
            .Where(x =>
                (x.SenderId == currentUserId && x.ReceiverId == userId) ||
                (x.SenderId == userId && x.ReceiverId == currentUserId))
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return messages.Select(x => x.ToMessageResponse()).ToList();
    }

    public async Task<UnreadCountResponseDto> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var count = await dbContext.Messages.CountAsync(
            x => x.ReceiverId == currentUserId && !x.IsRead,
            cancellationToken);

        return new UnreadCountResponseDto
        {
            Count = count
        };
    }
}
