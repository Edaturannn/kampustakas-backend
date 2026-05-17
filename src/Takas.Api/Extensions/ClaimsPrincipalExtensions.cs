using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Takas.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetUserIdOrNull(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                    ?? principal.FindFirstValue("sub");

        return int.TryParse(value, out var userId) ? userId : null;
    }

    public static int GetRequiredUserId(this ClaimsPrincipal principal)
    {
        return principal.GetUserIdOrNull()
               ?? throw new InvalidOperationException("Token içindeki kullanıcı kimliği bulunamadı.");
    }

    public static string? GetClaimValue(this ClaimsPrincipal principal, params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var value = principal.FindFirstValue(claimType);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}
