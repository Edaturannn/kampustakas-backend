using Takas.Api.DTOs.Offers;

namespace Takas.Api.Services.Interfaces;

public interface IOfferService
{
    Task<OfferResponseDto> CreateOfferAsync(CreateOfferRequestDto request, CancellationToken cancellationToken = default);
    Task<List<OfferResponseDto>> GetIncomingOffersAsync(CancellationToken cancellationToken = default);
    Task<List<OfferResponseDto>> GetOutgoingOffersAsync(CancellationToken cancellationToken = default);
    Task<OfferResponseDto> AcceptOfferAsync(int id, CancellationToken cancellationToken = default);
    Task<OfferResponseDto> RejectOfferAsync(int id, CancellationToken cancellationToken = default);
    Task<OfferResponseDto> CancelOfferAsync(int id, CancellationToken cancellationToken = default);
}
