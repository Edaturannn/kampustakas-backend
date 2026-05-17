using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Auth;
using Takas.Api.DTOs.Common;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(request, cancellationToken);
        return Success(response, "Kayıt işlemi başarılı.", StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        return Success(response, "Giriş işlemi başarılı.");
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<AuthUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthUserResponseDto>>> GetMe(CancellationToken cancellationToken)
    {
        var response = await authService.GetMeAsync(cancellationToken);
        return Success(response, "Kullanıcı bilgileri getirildi.");
    }
}
