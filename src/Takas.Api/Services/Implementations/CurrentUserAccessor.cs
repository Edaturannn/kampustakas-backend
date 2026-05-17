using Takas.Api.Extensions;
using Takas.Api.Entities;
using Takas.Api.Helpers;
using Takas.Api.Services.Interfaces;

namespace Takas.Api.Services.Implementations;

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    public User GetRequiredCurrentUser()
    {
        var user = httpContextAccessor.HttpContext?.Items[HttpContextItemKeys.CurrentUser] as User;
        return user ?? throw new UnauthorizedAppException("Oturum bilgisi doğrulanamadı.");
    }

    public int GetRequiredCurrentUserId()
    {
        return GetRequiredCurrentUser().Id;
    }

    public int? GetCurrentUserIdOrNull()
    {
        if (httpContextAccessor.HttpContext?.Items[HttpContextItemKeys.CurrentUser] is User user)
        {
            return user.Id;
        }

        var principal = httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return principal.GetUserIdOrNull();
    }
}
