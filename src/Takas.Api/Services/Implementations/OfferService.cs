using Takas.Api.Data;
using Takas.Api.DTOs.Offers;
using Takas.Api.Entities;
using Takas.Api.Enums;
using Takas.Api.Extensions;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class OfferService(
    AppDbContext dbContext,
    ICurrentUserAccessor currentUserAccessor,
    INotificationService notificationService) : IOfferService
{
    public async Task<OfferResponseDto> CreateOfferAsync(CreateOfferRequestDto request, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var product = await dbContext.Products
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.Status == ProductStatus.Available, cancellationToken)
            ?? throw new NotFoundException("Teklif yapılacak ürün bulunamadı.");

        if (product.OwnerId == currentUserId)
        {
            throw new ConflictException("Kendi ürününüze teklif gönderemezsiniz.");
        }

        var duplicateOfferExists = await dbContext.Offers.AnyAsync(
            x => x.ProductId == request.ProductId &&
                 x.SenderId == currentUserId &&
                 x.Status == OfferStatus.Pending,
            cancellationToken);

        if (duplicateOfferExists)
        {
            throw new ConflictException("Bu ürün için zaten bekleyen bir teklifiniz var.");
        }

        if (request.OfferedProductId.HasValue)
        {
            var offeredProduct = await dbContext.Products.FirstOrDefaultAsync(
                x => x.Id == request.OfferedProductId.Value &&
                     x.OwnerId == currentUserId &&
                     x.Status == ProductStatus.Available,
                cancellationToken);

            if (offeredProduct is null)
            {
                throw new BadRequestException("Teklifte kullanılacak ürün size ait değil veya uygun durumda değil.");
            }
        }

        var offer = new Offer
        {
            ProductId = request.ProductId,
            SenderId = currentUserId,
            OfferedProductId = request.OfferedProductId,
            CashAmount = request.CashAmount,
            Message = request.Message?.Trim(),
            Status = OfferStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Offers.Add(offer);
        await dbContext.SaveChangesAsync(cancellationToken);

        var savedOffer = await BuildOfferQuery()
            .FirstAsync(x => x.Id == offer.Id, cancellationToken);

        await notificationService.CreateAsync(
            product.OwnerId,
            "Yeni teklif",
            $"{savedOffer.Sender.FullName}, \"{product.Title}\" ürünü için size teklif gönderdi.",
            cancellationToken);

        return savedOffer.ToOfferResponse();
    }

    public async Task<List<OfferResponseDto>> GetIncomingOffersAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var offers = await BuildOfferQuery()
            .Where(x => x.Product.OwnerId == currentUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return offers.Select(x => x.ToOfferResponse()).ToList();
    }

    public async Task<List<OfferResponseDto>> GetOutgoingOffersAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var offers = await BuildOfferQuery()
            .Where(x => x.SenderId == currentUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return offers.Select(x => x.ToOfferResponse()).ToList();
    }

    public async Task<OfferResponseDto> AcceptOfferAsync(int id, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var offer = await BuildOfferQuery(true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Teklif bulunamadı.");

        if (offer.Product.OwnerId != currentUserId)
        {
            throw new ForbiddenException("Sadece kendi ürününüze gelen teklifi kabul edebilirsiniz.");
        }

        if (offer.Status != OfferStatus.Pending)
        {
            throw new ConflictException("Sadece bekleyen teklifler kabul edilebilir.");
        }

        offer.Status = OfferStatus.Accepted;
        offer.Product.Status = ProductStatus.Reserved;

        dbContext.DeliveryVerifications.Add(new DeliveryVerification
        {
            OfferId = offer.Id,
            Code = Random.Shared.Next(100000, 999999).ToString(),
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow
        });

        var rejectedOffers = await dbContext.Offers
            .Where(x => x.ProductId == offer.ProductId &&
                        x.Id != offer.Id &&
                        x.Status == OfferStatus.Pending)
            .ToListAsync(cancellationToken);

        foreach (var rejectedOffer in rejectedOffers)
        {
            rejectedOffer.Status = OfferStatus.Rejected;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        await notificationService.CreateAsync(
            offer.SenderId,
            "Teklif kabul edildi",
            $"\"{offer.Product.Title}\" ürünü için gönderdiğiniz teklif kabul edildi.",
            cancellationToken);

        foreach (var rejectedOffer in rejectedOffers)
        {
            await notificationService.CreateAsync(
                rejectedOffer.SenderId,
                "Teklif sonuçlandı",
                $"\"{offer.Product.Title}\" ürünü için gönderdiğiniz teklif başka bir kullanıcı lehine sonuçlandı.",
                cancellationToken);
        }

        return offer.ToOfferResponse();
    }

    public async Task<OfferResponseDto> RejectOfferAsync(int id, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var offer = await BuildOfferQuery(true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Teklif bulunamadı.");

        if (offer.Product.OwnerId != currentUserId)
        {
            throw new ForbiddenException("Sadece kendi ürününüze gelen teklifi reddedebilirsiniz.");
        }

        if (offer.Status != OfferStatus.Pending)
        {
            throw new ConflictException("Sadece bekleyen teklifler reddedilebilir.");
        }

        offer.Status = OfferStatus.Rejected;
        await dbContext.SaveChangesAsync(cancellationToken);

        await notificationService.CreateAsync(
            offer.SenderId,
            "Teklif reddedildi",
            $"\"{offer.Product.Title}\" ürünü için gönderdiğiniz teklif reddedildi.",
            cancellationToken);

        return offer.ToOfferResponse();
    }

    public async Task<OfferResponseDto> CancelOfferAsync(int id, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        var offer = await BuildOfferQuery(true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Teklif bulunamadı.");

        if (offer.SenderId != currentUserId)
        {
            throw new ForbiddenException("Sadece kendi gönderdiğiniz teklifi iptal edebilirsiniz.");
        }

        if (offer.Status != OfferStatus.Pending)
        {
            throw new ConflictException("Sadece bekleyen teklifler iptal edilebilir.");
        }

        offer.Status = OfferStatus.Cancelled;
        await dbContext.SaveChangesAsync(cancellationToken);

        return offer.ToOfferResponse();
    }

    private IQueryable<Offer> BuildOfferQuery(bool tracking = false)
    {
        var query = tracking ? dbContext.Offers.AsQueryable() : dbContext.Offers.AsNoTracking();

        return query
            .Include(x => x.Product)
                .ThenInclude(x => x.Owner)
            .Include(x => x.Sender)
            .Include(x => x.OfferedProduct);
    }
}
