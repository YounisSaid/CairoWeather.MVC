using CairoWeather.Core.Services;
using CairoWeather.Data.DbContexts;
using CairoWeather.MVC.Data.Seeding;
using CairoWeather.Services;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Register the Database Context (SQL Server)
        builder.Services.AddDbContext<EnergyDbContext>(options =>
      options.UseSqlServer(
          builder.Configuration.GetConnectionString("DefaultConnection"),
          sqlServerOptionsAction: sqlOptions =>
          {
              // This is the magic line that fixes the crash
              sqlOptions.EnableRetryOnFailure(
                  maxRetryCount: 5,
                  maxRetryDelay: TimeSpan.FromSeconds(30),
                  errorNumbersToAdd: null);
          }));

        // 2. Register Analytics Services (Scoped)
        // Note: Make sure IWeatherAnalyticsService matches your actual interface name!
        builder.Services.AddScoped<IWeatherAnalyticsService, WeatherAnalyticsService>();

        // 3. Register the Data Seeder
        builder.Services.AddScoped<DataSeeder>();

        // Add services to the container.
        builder.Services.AddControllersWithViews(); // For HTML Views
        builder.Services.AddControllers();          // ADD THIS: Explicit support for API Controllers

        var app = builder.Build();

        // 4. Trigger Data Seeding and Migrations on Startup
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<EnergyDbContext>();
                var env = services.GetRequiredService<IWebHostEnvironment>();

                // Auto-apply any pending database migrations
                await context.Database.MigrateAsync();

                // Path to our single source of truth (Hourly CSV)
                string csvPath = Path.Combine(env.WebRootPath, "open-meteo-30.05N31.19E22m (2).csv");

                // Check if file exists before seeding to avoid silent errors
                if (File.Exists(csvPath))
                {
                    var seeder = services.GetRequiredService<DataSeeder>();
                    await seeder.SeedAllDataAsync(csvPath);
                }
                else
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogWarning("Seeding skipped: CSV file not found at {Path}", csvPath);
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during database migration or seeding.");
            }
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
              name: "default",
              pattern: "{controller=Dashboard}/{action=Hourly}/{id?}");

        // ADD THIS: API Attribute Routing
        app.MapControllers();

        await app.RunAsync();
    }
}