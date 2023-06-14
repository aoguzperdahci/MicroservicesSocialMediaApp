using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IConfiguration configuration;
if (builder.Environment.IsDevelopment())
{
    Console.WriteLine(">>>> Dev");
    configuration = new ConfigurationBuilder()
                            .AddJsonFile("ocelot.development.json")
                            .Build();
}
else
{
    configuration = new ConfigurationBuilder()
                            .AddJsonFile("ocelot.production.json")
                            .Build();
}

builder.Services.AddOcelot(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseOcelot().Wait();

app.MapControllers();

app.Run();
