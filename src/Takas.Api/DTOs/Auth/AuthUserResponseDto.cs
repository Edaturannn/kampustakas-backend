using Takas.Api.Enums;

namespace Takas.Api.DTOs.Auth;

public class AuthUserResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Department { get; set; }
    public string? Campus { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; }
    public double Rating { get; set; }
    public int SuccessfulSwaps { get; set; }
    public DateTime CreatedAt { get; set; }
}
