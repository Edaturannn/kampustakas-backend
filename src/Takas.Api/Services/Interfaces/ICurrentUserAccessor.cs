using Takas.Api.Entities;

namespace Takas.Api.Services.Interfaces;

public interface ICurrentUserAccessor
{
    User GetRequiredCurrentUser();
    int GetRequiredCurrentUserId();
    int? GetCurrentUserIdOrNull();
}
