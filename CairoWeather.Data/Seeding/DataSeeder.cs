using CairoWeather.Core.Models;
using CairoWeather.Data.DbContexts;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;

namespace CairoWeather.MVC.Data.Seeding
{
    public class DataSeeder
    {
        private readonly EnergyDbContext _context;
        private readonly string _connectionString;

        public DataSeeder(EnergyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task SeedAllDataAsync(string hourlyCsvPath, string dailyCsvPath)
        {
            // 1. Check if data already exists to avoid duplicates
            if (await _context.HourlyReadings.AnyAsync() || await _context.DailyReadings.AnyAsync())
                return;

            // 2. Process Hourly Data (~87k rows)
            var hourlyData = ParseCsv<HourlyReading>(hourlyCsvPath, skipLines: 3);
            await BulkInsertAsync(hourlyData, "HourlyReadings");

            // 3. Process Daily Data (~3.6k rows)
            var dailyData = ParseCsv<DailyReading>(dailyCsvPath, skipLines: 3);
            await BulkInsertAsync(dailyData, "DailyReadings");
        }

        private List<T> ParseCsv<T>(string filePath, int skipLines)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                // Normalizes headers like "temperature_2m (°C)" to "temperature2m" to match class properties
                PrepareHeaderForMatch = args => args.Header.ToLower()
                    .Split(' ')[0]
                    .Replace("_", "")
                    .Replace("-", ""),
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(filePath);
            // Skip Open-Meteo metadata lines
            for (int i = 0; i < skipLines; i++) reader.ReadLine();

            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<T>().ToList();
        }

        private async Task BulkInsertAsync<T>(IEnumerable<T> data, string tableName)
        {
            using var table = new DataTable();
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id") // Let SQL handle the Identity Primary Key
                .ToArray();

            foreach (var prop in properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                table.Columns.Add(prop.Name, type);
            }

            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item) ?? DBNull.Value).ToArray();
                table.Rows.Add(values);
            }

            using var bulkCopy = new SqlBulkCopy(_connectionString);
            bulkCopy.DestinationTableName = tableName;
            bulkCopy.BatchSize = 5000;

            // Map columns explicitly to ensure data lands in the correct SQL columns
            foreach (var prop in properties)
                bulkCopy.ColumnMappings.Add(prop.Name, prop.Name);

            await bulkCopy.WriteToServerAsync(table);
        }
    }
}