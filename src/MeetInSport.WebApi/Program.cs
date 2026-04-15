using Microsoft.EntityFrameworkCore;
using MeetInSport.Infrastructure.Persistance;
using Microsoft.AspNetCore.Builder;
using MeetInSport.Infrastructure.Persistence;
using MeetInSport.WebApi.Middlewares;
using MeetInSport.Infrastructure.Persistence.Seeders;
using MeetInSport.Application;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS (Allow React frontend on port 3000 to communicate with this API)

builder.Services.AddInfrastructure(); // Register infrastructure services.
builder.Services.AddApplication();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // React frontend origin (Look at again to make sure url is correct)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

try
{
    using (var scope = app.Services.CreateScope()) // Temporary scope --> just to get the seeder, run it and close it.
    {
        var seeder = scope.ServiceProvider.GetRequiredService<DataBaseSeeder>();
        await seeder.SeedAsync();
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error during database seeding");
    // Continue without crashing
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP ---->> HTTPS
app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();