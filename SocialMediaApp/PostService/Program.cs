using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using PostService.Data;
using PostService.MessageBus;
using PostService.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddDbContext<PostDataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();

services.AddOpenApiDocument(options =>
{
    options.Title = "Post Service";
    options.DocumentName = "post-service";
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

builder.Services.AddScoped<IPostService, PostService.Services.PostService>();
builder.Services.AddHttpClient();

services.AddDbContext<PostDataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddCors(options => options.AddPolicy(name: "AcceptAll",
    policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));

services.AddSingleton<IMessageBusClient, MessageBusClient>();
services.AddSingleton<IEventProcessor, EventProcessor>();
services.AddHostedService<MessageBusClient>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(options =>
    {
        options.Path = "v1/openapi/post-service.yaml";
        options.DocumentName = "post-service";
    });

    app.UseSwaggerUi3(options =>
    {
        options.Path = "/openapi";
        options.DocumentPath = "/v1/openapi/post-service.yaml";
    });
}
app.UseCors("AcceptAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run(); 
