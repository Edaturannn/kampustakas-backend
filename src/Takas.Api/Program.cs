using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Takas.Api.Data;
using Takas.Api.DTOs.Common;
using Takas.Api.Extensions;
using Takas.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

DatabaseConnectionStringResolver.ApplyDatabaseUrlOverride(builder.Configuration);

builder.Services
    .AddApplicationDatabase(builder.Configuration)
    .AddApplicationServices()
    .AddApplicationAuthentication(builder.Configuration)
    .AddApplicationSwagger();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(error =>
                    string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? $"{x.Key} alanı geçersiz."
                        : error.ErrorMessage))
                .Distinct()
                .ToList();

            var response = ApiResponse<object?>.FailResponse(
                "Gönderilen veriler doğrulanamadı.",
                StatusCodes.Status400BadRequest,
                errors);

            return new BadRequestObjectResult(response);
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "https://kampustakas.com",
                "https://www.kampustakas.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

var isEfDesignTime = AppDomain.CurrentDomain.GetData("EF.IsDesignTime") as bool? ?? false;
var skipDatabaseMigrationOnStartup = string.Equals(
    Environment.GetEnvironmentVariable("SKIP_DB_MIGRATION_ON_STARTUP"),
    "true",
    StringComparison.OrdinalIgnoreCase);

if (!isEfDesignTime && !skipDatabaseMigrationOnStartup)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;

    if (response.HasStarted)
    {
        return;
    }

    response.ContentType = "application/json";

    var payload = response.StatusCode switch
    {
        StatusCodes.Status401Unauthorized => ApiResponse<object?>.FailResponse(
            "Bu işlem için giriş yapmanız gerekiyor.",
            StatusCodes.Status401Unauthorized),

        StatusCodes.Status403Forbidden => ApiResponse<object?>.FailResponse(
            "Bu işlem için yetkiniz bulunmuyor.",
            StatusCodes.Status403Forbidden),

        StatusCodes.Status404NotFound => ApiResponse<object?>.FailResponse(
            "İstenen kaynak bulunamadı.",
            StatusCodes.Status404NotFound),

        _ => ApiResponse<object?>.FailResponse(
            "İşlem tamamlanamadı.",
            response.StatusCode)
    };

    await response.WriteAsJsonAsync(payload);
});

var applicationUrls = builder.Configuration["ASPNETCORE_URLS"]
    ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

if (!string.IsNullOrWhiteSpace(applicationUrls) &&
    applicationUrls.Contains("https://", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();

app.MapControllers();

if (!isEfDesignTime && !skipDatabaseMigrationOnStartup)
{
    app.Run();
}

public partial class Program;
