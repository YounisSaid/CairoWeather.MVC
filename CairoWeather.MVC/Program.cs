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
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // 2. Register Analytics Services (Scoped)
        builder.Services.AddScoped<IWeatherAnalyticsService, WeatherAnalyticsService>();

        // 3. Register the Data Seeder
        builder.Services.AddScoped<DataSeeder>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // 4. Trigger Data Seeding on Startup
        // This scope ensures the database is populated before the web server starts handling requests
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                // Use WebRootPath so it looks inside wwwroot!
                var env = services.GetRequiredService<IWebHostEnvironment>();
                string hourlyPath = Path.Combine(env.WebRootPath, "open-meteo-30.05N31.19E22m (2).csv");
                string dailyPath = Path.Combine(env.WebRootPath, "open-meteo-30.05N31.19E22m (4).csv");

                // Force the seeder to run
                var seeder = services.GetRequiredService<DataSeeder>();
                seeder.SeedAllDataAsync(hourlyPath, dailyPath).Wait();

            }
            catch (Exception ex)
            {
                // Log errors if seeding fails
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during the initial data seeding process.");
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
            pattern: "{controller=Dashboard}/{action=Index}/{id?}");

        app.Run();
    }
}