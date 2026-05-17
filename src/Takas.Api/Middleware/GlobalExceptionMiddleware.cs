using Microsoft.EntityFrameworkCore;
using Npgsql;
using Takas.Api.DTOs.Common;
using Takas.Api.Helpers;

namespace Takas.Api.Middleware;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "İstek işlenirken hata oluştu.");
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            AppException appException => ApiResponse<object?>.FailResponse(
                appException.Message,
                appException.StatusCode,
                appException.Errors),
            BadHttpRequestException => ApiResponse<object?>.FailResponse(
                "İstek gövdesi okunamadı veya geçersiz veri gönderildi.",
                StatusCodes.Status400BadRequest),
            DbUpdateException dbUpdateException when dbUpdateException.InnerException is PostgresException postgresException &&
                                                   postgresException.SqlState == PostgresErrorCodes.UniqueViolation =>
                ApiResponse<object?>.FailResponse(
                    "Aynı bilgilerle kayıt zaten mevcut.",
                    StatusCodes.Status409Conflict),
            _ => ApiResponse<object?>.FailResponse(
                "Sunucu tarafında beklenmeyen bir hata oluştu.",
                StatusCodes.Status500InternalServerError)
        };

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}
