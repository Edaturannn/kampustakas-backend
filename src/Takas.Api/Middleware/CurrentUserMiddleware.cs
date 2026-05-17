using Takas.Api.Data;
using Takas.Api.Extensions;
using Takas.Api.Helpers;

namespace Takas.Api.Middleware;

public class CurrentUserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            context.Items[HttpContextItemKeys.CurrentUser] is null)
        {
            int userId;

            try
            {
                userId = context.User.GetRequiredUserId();
            }
            catch (InvalidOperationException exception)
            {
                throw new UnauthorizedAppException(exception.Message);
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, context.RequestAborted)
                       ?? throw new UnauthorizedAppException("Kullanıcı oturumu bulunamadı.");

            context.Items[HttpContextItemKeys.CurrentUser] = user;
        }

        await next(context);
    }
}
