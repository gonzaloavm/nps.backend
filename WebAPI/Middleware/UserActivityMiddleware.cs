using Domain.Repositories;
using System.Security.Claims;

namespace WebAPI.Middleware
{
    public class UserActivityMiddleware
    {
        private readonly RequestDelegate _next;

        public UserActivityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    var user = await userRepository.GetByIdAsync(userId);
                    if (user != null)
                    {
                        user.LastActivity = DateTime.UtcNow;
                        await userRepository.UpdateAsync(user);
                    }
                }
            }
            await _next(context);
        }
    }
}
