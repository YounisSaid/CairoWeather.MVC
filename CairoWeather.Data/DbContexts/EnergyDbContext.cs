using CairoWeather.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CairoWeather.Data.DbContexts
{
    public class EnergyDbContext : DbContext
    {
        public EnergyDbContext(DbContextOptions<EnergyDbContext> options) : base(options)
        {
        }

        public DbSet<HourlyReading> HourlyReadings { get; set; } = null!;
        public DbSet<TmyReading> TmyReadings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // الميزة السحرية TPC: تفصل الجدولين تماماً وتمنع دمجهم
            modelBuilder.Entity<HourlyReading>().UseTpcMappingStrategy();

            // 1. إعدادات جدول الداتا التاريخية
            modelBuilder.Entity<HourlyReading>()
                .ToTable("HourlyReadings")
                .HasIndex(h => h.Time)
                .IsUnique();

            // 2. إعدادات جدول سنة الـ TMY
            modelBuilder.Entity<TmyReading>()
                .ToTable("TmyReadings")
                .HasIndex(h => h.Time)
                .IsUnique();
        }
    }
}