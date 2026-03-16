using Domain.Repositories;

namespace WebAPI.Middleware
{
    public class SessionActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public SessionActivityMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Obtener el SessionId desde los claims
                var sessionIdClaim = context.User.FindFirst("SessionId")?.Value;
                if (int.TryParse(sessionIdClaim, out var sessionId))
                {
                    // Disparamos la actualización sin esperar
                    _ = Task.Run(async () =>
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                        await repo.UpdateSessionLastActivityAsync(sessionId, DateTime.Now);
                    });
                }
            }
            await _next(context);
        }
    }
}
