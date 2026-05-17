using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Takas.Api.DTOs.Admin;
using Takas.Api.DTOs.Common;
using Takas.Api.DTOs.Notifications;
using Takas.Api.DTOs.Offers;
using Takas.Api.DTOs.Products;
using Takas.Api.DTOs.Reports;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin")]
public class AdminController(
    IAdminService adminService,
    IReportService reportService) : BaseApiController
{
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<AdminDashboardResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AdminDashboardResponseDto>>> GetDashboard(CancellationToken cancellationToken)
    {
        var response = await adminService.GetDashboardAsync(cancellationToken);
        return Success(response, "Yönetim paneli özeti getirildi.");
    }

    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<List<AdminUserResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AdminUserResponseDto>>>> GetUsers(CancellationToken cancellationToken)
    {
        var response = await adminService.GetUsersAsync(cancellationToken);
        return Success(response, "Kullanıcılar getirildi.");
    }

    [HttpGet("users/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<AdminUserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AdminUserResponseDto>>> GetUserById(int id, CancellationToken cancellationToken)
    {
        var response = await adminService.GetUserByIdAsync(id, cancellationToken);
        return Success(response, "Kullanıcı detayı getirildi.");
    }

    [HttpGet("products")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ProductResponseDto>>>> GetProducts(CancellationToken cancellationToken)
    {
        var response = await adminService.GetProductsAsync(cancellationToken);
        return Success(response, "Tüm ürünler getirildi.");
    }

    [HttpGet("offers")]
    [ProducesResponseType(typeof(ApiResponse<List<OfferResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<OfferResponseDto>>>> GetOffers(CancellationToken cancellationToken)
    {
        var response = await adminService.GetOffersAsync(cancellationToken);
        return Success(response, "Tüm teklifler getirildi.");
    }

    [HttpGet("notifications")]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<NotificationResponseDto>>>> GetNotifications(CancellationToken cancellationToken)
    {
        var response = await adminService.GetNotificationsAsync(cancellationToken);
        return Success(response, "Tüm bildirimler getirildi.");
    }

    [HttpGet("reports")]
    [ProducesResponseType(typeof(ApiResponse<List<ReportResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ReportResponseDto>>>> GetReports(CancellationToken cancellationToken)
    {
        var response = await reportService.GetAdminReportsAsync(cancellationToken);
        return Success(response, "Şikayet kayıtları getirildi.");
    }
}
