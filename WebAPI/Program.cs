using Application.Behaviors;
using Application.Contracts;
using Application.Features.Auth.Commands;
using Domain.Common.Interfaces;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Data;
using System.Reflection;
using System.Text;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

// OpenAPI + Scalar
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "NPS Backend API";
        document.Info.Version = "v1";
        document.Info.Description = "API nativa para la gestión de encuestas NPS.";
        document.Info.Contact = new OpenApiContact
        {
            Name = "Soporte Técnico",
            Email = "dev@nps.com"
        };
        return Task.CompletedTask;
    });

});

// Repositorios y servicios
var infraAssembly = Assembly.Load("Infrastructure");
builder.Services.AddMappedComponents<IRepositoryBase>(infraAssembly);
builder.Services.AddMappedComponents<IServiceBase>(infraAssembly);

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

// FluentValidation
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly);

    // Registra el Behavior
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(Application.AssemblyReference).Assembly);

// Dapper Context
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IDbConnection>(sp =>
    sp.GetRequiredService<DapperContext>().CreateConnection());

// Inicializador DB
builder.Services.AddScoped<DatabaseInitializer>();

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenService, TokenService>();

// HttpClient
builder.Services.AddHttpClient();

// Authentication JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("VoterOnly", policy => policy.RequireRole("Voter"));

#region CORS

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

#endregion

#region APP

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseMiddleware<ExceptionMiddleware>();

//app.UseHttpsRedirection();

app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();

app.UseMiddleware<SessionActivityMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion