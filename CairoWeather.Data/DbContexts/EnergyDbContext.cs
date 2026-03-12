using CairoWeather.Core.Models;
using Microsoft.EntityFrameworkCore;
namespace CairoWeather.Data.DbContexts
{
    public class EnergyDbContext : DbContext
    {
        public EnergyDbContext(DbContextOptions<EnergyDbContext> options) : base(options)
        {
        }

        public DbSet<HourlyReading> HourlyReadings { get; set; }
        public DbSet<DailyReading> DailyReadings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexing for high-speed lookups in Section 1 (Hourly)
            modelBuilder.Entity<HourlyReading>()
                .HasIndex(h => h.Time);

            // Indexing for high-speed aggregation in Section 2 & 3 (Yearly/Decade)
            modelBuilder.Entity<DailyReading>()
                .HasIndex(d => d.Date);

            // Mapping doubles to float in SQL Server for scientific precision
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(double)))
            {
                property.SetColumnType("float");
            }
        }
    }
}