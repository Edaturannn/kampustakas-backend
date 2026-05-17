using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Common;

namespace Takas.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Success<T>(T data, string message, int statusCode = StatusCodes.Status200OK)
    {
        return StatusCode(statusCode, ApiResponse<T>.SuccessResponse(data, message, statusCode));
    }

    protected ActionResult<ApiResponse<object?>> SuccessMessage(string message, int statusCode = StatusCodes.Status200OK)
    {
        return StatusCode(statusCode, ApiResponse<object?>.SuccessResponse(null, message, statusCode));
    }
}
