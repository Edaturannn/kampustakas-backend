namespace Takas.Api.DTOs.Admin;

public class AdminDashboardResponseDto
{
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOffers { get; set; }
    public int PendingReports { get; set; }
    public int TotalNotifications { get; set; }
}
