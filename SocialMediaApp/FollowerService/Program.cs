using Neo4jClient;

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

