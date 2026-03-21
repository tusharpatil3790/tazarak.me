using MongoDB.Driver;
using PortfolioAPI.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("[startup] Building application (env=" + builder.Environment.EnvironmentName + ")...");

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:4200", "https://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Portfolio API",
        Version = "v1",
        Description = "API for Professional Portfolio Application",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Portfolio API Support"
        }
    });
});

// MongoDB Configuration
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
Console.WriteLine("[startup] MongoDB connection string read (length=" + (mongoConnectionString?.Length ?? 0) + ")");
try
{
    Console.WriteLine("[startup] Initializing MongoDB client...");
    var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
    mongoClientSettings.ConnectTimeout = TimeSpan.FromSeconds(10);
    mongoClientSettings.SocketTimeout = TimeSpan.FromSeconds(10);
    builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoClientSettings));
    Console.WriteLine("[startup] MongoDB client registered");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[startup][ERROR] Failed to create MongoDB client: {ex}");
    Console.Error.WriteLine($"[startup][ERROR] Connection string: {mongoConnectionString}");
    throw;
}
builder.Services.AddScoped<PortfolioAPI.Services.PortfolioService>();

// Email Service Configuration
builder.Services.AddScoped<PortfolioAPI.Services.IEmailService, PortfolioAPI.Services.EmailService>();

var app = builder.Build();

Console.WriteLine("[startup] Application built, registering middleware...");
Console.WriteLine($"[startup] Host created successfully. IsDevelopment={app.Environment.IsDevelopment()}, IsProduction={app.Environment.IsProduction()}");

// Use CORS
app.UseCors("AllowAngularApp");

// Configure Swagger/OpenAPI (enabled for all environments for testing)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");
    c.RoutePrefix = "swagger";
});

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/api/health", () => new { status = "ok", timestamp = DateTime.UtcNow })
    .WithName("Health");

// Commenting out lifetime handlers to prevent console signal handling issues
// app.Lifetime.ApplicationStarted.Register(() => 
// {
//     Console.WriteLine("[lifetime] ApplicationStarted handler called");
// });
// app.Lifetime.ApplicationStopping.Register(() => 
// {
//     Console.WriteLine("[lifetime] ApplicationStopping handler called");
//     var stackTrace = Environment.StackTrace;
//     Console.WriteLine($"[lifetime] StackTrace at stopping: {stackTrace}");
// });
// app.Lifetime.ApplicationStopped.Register(() => 
// {
//     Console.WriteLine("[lifetime] ApplicationStopped handler called");
// });

Console.WriteLine("[startup] Starting application...");
try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"[FATAL ERROR] Exception during app.RunAsync: {ex.GetType().Name}");
    Console.WriteLine($"[FATAL ERROR] Message: {ex.Message}");
    Console.WriteLine($"[FATAL ERROR] StackTrace: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"[FATAL ERROR] InnerException: {ex.InnerException.Message}");
    }
    throw;
}

