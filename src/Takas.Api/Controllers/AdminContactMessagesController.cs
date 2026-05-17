using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.ContactMessages;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/contact-messages")]
public class AdminContactMessagesController(IContactMessageService contactMessageService) : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ContactMessageResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ContactMessageResponseDto>>>> GetMessages(CancellationToken cancellationToken)
    {
        var response = await contactMessageService.GetAdminMessagesAsync(cancellationToken);
        return Success(response, "Iletisim mesajlari getirildi.");
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ContactMessageResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ContactMessageResponseDto>>> GetMessageById(int id, CancellationToken cancellationToken)
    {
        var response = await contactMessageService.GetAdminMessageByIdAsync(id, cancellationToken);
        return Success(response, "Iletisim mesaji detayi getirildi.");
    }

    [HttpPut("{id:int}/reply")]
    [ProducesResponseType(typeof(ApiResponse<ContactMessageResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ContactMessageResponseDto>>> ReplyToMessage(
        int id,
        [FromBody] ReplyContactMessageRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await contactMessageService.ReplyToMessageAsync(id, request, cancellationToken);
        return Success(response, "Iletisim mesajina yanit verildi.");
    }

    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ApiResponse<ContactMessageResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ContactMessageResponseDto>>> UpdateStatus(
        int id,
        [FromBody] UpdateContactMessageStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await contactMessageService.UpdateStatusAsync(id, request, cancellationToken);
        return Success(response, "Iletisim mesaji durumu guncellendi.");
    }
}
