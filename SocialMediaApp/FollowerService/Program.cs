using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Neo4jClient;
using NSwag.Generation.Processors.Security;
using NSwag;
using System.Text;
using FollowerService.Services;
using FollowerService.MessageBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = "bolt://localhost:7687"; // Neo4j server URI
var username = "neo4j"; // Neo4j username
var password = "password"; // Neo4j password

// Create a new instance of GraphClient
var graphClient = new BoltGraphClient(new Uri(connectionString), username, password);

// Connect to the Neo4j database
graphClient.ConnectAsync();
builder.Services.AddSingleton<IGraphClient>(graphClient);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "Follower Service";
    options.DocumentName = "follower-service";
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddCors(options => options.AddPolicy(name: "AcceptAll",
    policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));

builder.Services.AddScoped<IFollowerService, Neo4jFollowerService>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(options =>
    {
        options.Path = "v1/openapi/follower-service.yaml";
        options.DocumentName = "follower-service";
    });

    app.UseSwaggerUi3(options =>
    {
        options.Path = "/openapi";
        options.DocumentPath = "/v1/openapi/follower-service.yaml";
    });
}
app.UseCors("AcceptAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

