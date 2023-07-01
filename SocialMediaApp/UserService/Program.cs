using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;
using NSwag;
using System.Text;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Helpers;
using UserService.Services;
using UserService.MessageBus;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
services.AddOpenApiDocument(options =>
{
    options.Title = "User Service";
    options.DocumentName = "user-service";
    options.OperationProcessors.Add(new OperationSecurityScopeProcessor("auth"));
    options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT Token"));
    options.DocumentProcessors.Add(new SecurityDefinitionAppender("auth", new NSwag.OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.Http,
        In = OpenApiSecurityApiKeyLocation.Header,
        Scheme = "bearer",
        BearerFormat = "jwt"
    }));
});

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("Secret:Key").Value)),
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateAudience = false
        };
    });

services.AddDbContext<UserDataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(options => options.AddPolicy(name: "AcceptAll",
    policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));

services.AddSingleton<IJwtUtils, JwtUtils>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUserService, UserService.Services.UserService>();
services.AddSingleton<IMessageBusClient, MessageBusClient>();
services.AddSingleton<IEventProcessor, EventProcessor>();
services.AddHostedService<MessageBusClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(options =>
    {
        options.Path = "v1/openapi/user-service.yaml";
        options.DocumentName = "user-service";
    });

    app.UseSwaggerUi3(options =>
    {
        options.Path = "/openapi";
        options.DocumentPath = "/v1/openapi/user-service.yaml";
    });
}
app.UseCors("AcceptAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
