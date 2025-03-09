using Microsoft.AspNetCore.Builder;
using hello_world_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register our JournalService
builder.Services.AddSingleton<JournalService>();
// Logging is already registered by default in .NET 6+

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Make sure the App_Data directory exists
var dataDir = builder.Configuration["JournalSettings:DataDirectory"] ?? "App_Data";
if (!Directory.Exists(dataDir))
{
    Directory.CreateDirectory(dataDir);
}

// Look for code similar to this which defines your API route configuration
app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action=Index}/{id?}");

app.Run();
