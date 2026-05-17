using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.Offers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class OffersController(IOfferService offerService) : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OfferResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<OfferResponseDto>>> CreateOffer(
        [FromBody] CreateOfferRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await offerService.CreateOfferAsync(request, cancellationToken);
        return Success(response, "Teklif oluşturuldu.", StatusCodes.Status201Created);
    }

    [HttpGet("incoming")]
    [ProducesResponseType(typeof(ApiResponse<List<OfferResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<OfferResponseDto>>>> GetIncomingOffers(CancellationToken cancellationToken)
    {
        var response = await offerService.GetIncomingOffersAsync(cancellationToken);
        return Success(response, "Gelen teklifler getirildi.");
    }

    [HttpGet("outgoing")]
    [ProducesResponseType(typeof(ApiResponse<List<OfferResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<OfferResponseDto>>>> GetOutgoingOffers(CancellationToken cancellationToken)
    {
        var response = await offerService.GetOutgoingOffersAsync(cancellationToken);
        return Success(response, "Giden teklifler getirildi.");
    }

    [HttpPost("{id:int}/accept")]
    [ProducesResponseType(typeof(ApiResponse<OfferResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OfferResponseDto>>> AcceptOffer(int id, CancellationToken cancellationToken)
    {
        var response = await offerService.AcceptOfferAsync(id, cancellationToken);
        return Success(response, "Teklif kabul edildi.");
    }

    [HttpPost("{id:int}/reject")]
    [ProducesResponseType(typeof(ApiResponse<OfferResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OfferResponseDto>>> RejectOffer(int id, CancellationToken cancellationToken)
    {
        var response = await offerService.RejectOfferAsync(id, cancellationToken);
        return Success(response, "Teklif reddedildi.");
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<OfferResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<OfferResponseDto>>> CancelOffer(int id, CancellationToken cancellationToken)
    {
        var response = await offerService.CancelOfferAsync(id, cancellationToken);
        return Success(response, "Teklif iptal edildi.");
    }
}
