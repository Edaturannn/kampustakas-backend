using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.ContactMessages;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Authorize]
[Route("api/contact/messages")]
public class ContactMessagesController(IContactMessageService contactMessageService) : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContactMessageResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<ContactMessageResponseDto>>> CreateMessage(
        [FromBody] CreateContactMessageRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await contactMessageService.CreateMessageAsync(request, cancellationToken);
        return Success(response, "Mesajiniz admin ekibine iletildi.", StatusCodes.Status201Created);
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<List<ContactMessageResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ContactMessageResponseDto>>>> GetMyMessages(CancellationToken cancellationToken)
    {
        var response = await contactMessageService.GetMyMessagesAsync(cancellationToken);
        return Success(response, "Kullanici iletisim mesajlari getirildi.");
    }
}
