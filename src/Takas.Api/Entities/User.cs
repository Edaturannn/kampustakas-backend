using Takas.Api.Enums;

namespace Takas.Api.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Campus { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public double Rating { get; set; }
    public int SuccessfulSwaps { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Offer> SentOffers { get; set; } = new List<Offer>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Report> CreatedReports { get; set; } = new List<Report>();
    public ICollection<Report> ReportsAboutUser { get; set; } = new List<Report>();
}
