using Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
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
            ProblemDetails problem;

            if (ex is ValidationException validationEx)
            {
                // 1. Manejo específico para errores de FluentValidation (400)
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var errors = validationEx.Errors.Select(f => new Error(
                    "VALIDATION_ERROR",
                    f.ErrorMessage,
                    f.PropertyName));

                problem = ProblemDetailsMapper.ToProblemDetails(errors, context.Response.StatusCode, "Validation Error");
            }
            else
            {
                // 2. Manejo para errores inesperados del servidor (500)
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var error = new Error("INTERNAL_SERVER_ERROR", _env.IsDevelopment() ? ex.Message : "Error interno en el servidor.");
                problem = ProblemDetailsMapper.ToProblemDetails(new[] { error }, context.Response.StatusCode, "Internal Server Error");

                if (_env.IsDevelopment())
                {
                    problem.Detail = ex.ToString();
                }
            }

            problem.Instance = context.Request.Path;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(problem, options);

            await context.Response.WriteAsync(json);
        }
    }
}