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
                var seeder = services.GetRequiredService<DataSeeder>();
                var env = services.GetRequiredService<IWebHostEnvironment>();

                // Define paths to your CSV files (assuming they are in App_Data or similar)
                // You should place your CSV files in a folder named 'Data' in your Web project
                string hourlyPath = Path.Combine(env.ContentRootPath, "Data", "open-meteo-30.05N31.19E22m (2).csv");
                string dailyPath = Path.Combine(env.ContentRootPath, "Data", "open-meteo-30.05N31.19E22m (4).csv");

                // Run the bulk seeder
                await seeder.SeedAllDataAsync(hourlyPath, dailyPath);
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