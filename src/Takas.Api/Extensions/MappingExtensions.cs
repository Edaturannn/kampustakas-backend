using Takas.Api.DTOs.Admin;
using Takas.Api.DTOs.Auth;
using Takas.Api.DTOs.Categories;
using Takas.Api.DTOs.ContactMessages;
using Takas.Api.DTOs.Favorites;
using Takas.Api.DTOs.Messages;
using Takas.Api.DTOs.Notifications;
using Takas.Api.DTOs.Offers;
using Takas.Api.DTOs.Products;
using Takas.Api.DTOs.Reports;
using Takas.Api.Entities;

namespace Takas.Api.Extensions;

public static class MappingExtensions
{
    public static AuthUserResponseDto ToAuthUserResponse(this User user)
    {
        return new AuthUserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Department = user.Department,
            Campus = user.Campus,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            Rating = user.Rating,
            SuccessfulSwaps = user.SuccessfulSwaps,
            CreatedAt = user.CreatedAt
        };
    }

    public static AdminUserResponseDto ToAdminUserResponse(this User user)
    {
        return new AdminUserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Department = user.Department,
            Campus = user.Campus,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            Rating = user.Rating,
            SuccessfulSwaps = user.SuccessfulSwaps,
            CreatedAt = user.CreatedAt
        };
    }

    public static CategoryResponseDto ToCategoryResponse(this Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public static ProductResponseDto ToProductResponse(this Product product, bool isFavorited = false)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            OwnerId = product.OwnerId,
            OwnerName = product.Owner.FullName,
            OwnerAvatarUrl = product.Owner.AvatarUrl,
            OwnerRating = product.Owner.Rating,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            Title = product.Title,
            Description = product.Description,
            Condition = product.Condition,
            EstimatedMinPrice = product.EstimatedMinPrice,
            EstimatedMaxPrice = product.EstimatedMaxPrice,
            Campus = product.Campus,
            Status = product.Status,
            ViewCount = product.ViewCount,
            CreatedAt = product.CreatedAt,
            IsFavorited = isFavorited,
            Images = product.ProductImages
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.Id)
                .Select(x => x.ToProductImageResponse())
                .ToList()
        };
    }

    public static ProductImageResponseDto ToProductImageResponse(this ProductImage productImage)
    {
        return new ProductImageResponseDto
        {
            Id = productImage.Id,
            ImageUrl = productImage.ImageUrl,
            IsMain = productImage.IsMain
        };
    }

    public static OfferResponseDto ToOfferResponse(this Offer offer)
    {
        return new OfferResponseDto
        {
            Id = offer.Id,
            ProductId = offer.ProductId,
            ProductTitle = offer.Product.Title,
            ProductOwnerId = offer.Product.OwnerId,
            ProductOwnerName = offer.Product.Owner.FullName,
            SenderId = offer.SenderId,
            SenderName = offer.Sender.FullName,
            OfferedProductId = offer.OfferedProductId,
            OfferedProductTitle = offer.OfferedProduct?.Title,
            CashAmount = offer.CashAmount,
            Message = offer.Message,
            Status = offer.Status,
            CreatedAt = offer.CreatedAt
        };
    }

    public static MessageResponseDto ToMessageResponse(this Message message)
    {
        return new MessageResponseDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderName = message.Sender.FullName,
            ReceiverId = message.ReceiverId,
            ReceiverName = message.Receiver.FullName,
            ProductId = message.ProductId,
            Content = message.Content,
            IsRead = message.IsRead,
            CreatedAt = message.CreatedAt
        };
    }

    public static NotificationResponseDto ToNotificationResponse(this Notification notification)
    {
        return new NotificationResponseDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }

    public static ContactMessageResponseDto ToContactMessageResponse(this ContactMessage contactMessage)
    {
        return new ContactMessageResponseDto
        {
            Id = contactMessage.Id,
            UserId = contactMessage.UserId,
            UserFullName = contactMessage.User.FullName,
            UserEmail = contactMessage.User.Email,
            Subject = contactMessage.Subject,
            Message = contactMessage.Message,
            Status = contactMessage.Status,
            AdminReply = contactMessage.AdminReply,
            CreatedAt = contactMessage.CreatedAt,
            RepliedAt = contactMessage.RepliedAt
        };
    }

    public static FavoriteResponseDto ToFavoriteResponse(this Favorite favorite)
    {
        return new FavoriteResponseDto
        {
            Id = favorite.Id,
            ProductId = favorite.ProductId,
            ProductTitle = favorite.Product.Title,
            CategoryName = favorite.Product.Category.Name,
            OwnerId = favorite.Product.OwnerId,
            OwnerName = favorite.Product.Owner.FullName,
            MainImageUrl = favorite.Product.ProductImages
                .OrderByDescending(x => x.IsMain)
                .ThenBy(x => x.Id)
                .Select(x => x.ImageUrl)
                .FirstOrDefault(),
            Status = favorite.Product.Status,
            CreatedAt = favorite.Product.CreatedAt
        };
    }

    public static ReportResponseDto ToReportResponse(this Report report)
    {
        return new ReportResponseDto
        {
            Id = report.Id,
            ReporterId = report.ReporterId,
            ReporterName = report.Reporter.FullName,
            ReportedUserId = report.ReportedUserId,
            ReportedUserName = report.ReportedUser?.FullName,
            ProductId = report.ProductId,
            ProductTitle = report.Product?.Title,
            Reason = report.Reason,
            Status = report.Status,
            CreatedAt = report.CreatedAt
        };
    }
}
