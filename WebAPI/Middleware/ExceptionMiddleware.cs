using Domain.Common;
using Domain.Repositories;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common;

namespace WebAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var title = "Internal Server Error";
            var detail = _env.IsDevelopment() ? ex.ToString() : "Error interno en el servidor.";

            var problem = new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = context.Response.StatusCode,
                Instance = context.Request.Path
            };

            // Añadimos la lista de errores en Extensions para mantener compatibilidad con el cliente
            var error = new Error("INTERNAL_SERVER_ERROR", _env.IsDevelopment() ? ex.Message : "Error interno en el servidor.");
            problem.Extensions["errors"] = new[] { error };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problem, options);

            await context.Response.WriteAsync(json);
        }
    }
}