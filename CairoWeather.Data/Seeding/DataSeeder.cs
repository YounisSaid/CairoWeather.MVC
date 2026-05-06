using CairoWeather.Core.Models;
using CairoWeather.Data.DbContexts;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Globalization;
using System.Text.Json; // Added for deep cloning

namespace CairoWeather.MVC.Data.Seeding
{
    public class DataSeeder
    {
        private readonly EnergyDbContext _context;
        private readonly string _connectionString;

        // Constants for Calculations
        private const double R_d = 287.058; // Gas constant for dry air (J/(kg·K))
        private const double R_v = 461.495; // Gas constant for water vapor (J/(kg·K))
        private const double T_base_Crop = 10.0; // Base temp for Agricultural GDD

        public DataSeeder(EnergyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task SeedAllDataAsync(string hourlyCsvPath)
        {
            var hourlyData = ParseCsv<HourlyReading>(hourlyCsvPath, skipLines: 3);

            foreach (var hr in hourlyData)
            {
                // Safely extract raw base values from CSV
                double temp = hr.Temperature2m ?? 0;
                double dewPoint = hr.DewPoint2m ?? 0;
                double pressureHpa = hr.SurfacePressure ?? 1013.25;
                double windSpeed100m = hr.WindSpeed100m ?? 0;
                double cloudCover = hr.CloudCover ?? 0;

                // ====================================================================
                // 1. SOLAR RADIATION PHYSICS FALLBACK (Calculated because CSV is NULL)
                // ====================================================================
                int hour = hr.Time.Hour;
                double clearSky = 0;

                if (hour >= 6 && hour <= 18)
                {
                    double radian = (hour - 12) * Math.PI / 12;
                    clearSky = Math.Max(0, Math.Cos(radian) * 950);
                }
                hr.ClearSkyRadiation = Math.Round(clearSky, 2);

                double cloudEffect = 1 - (0.75 * Math.Pow(cloudCover / 100, 3));
                hr.ShortwaveRadiation = Math.Round(clearSky * cloudEffect, 2);
                hr.CloudRadiativeForcing = Math.Round(hr.ClearSkyRadiation.Value - hr.ShortwaveRadiation.Value, 2);

                // ====================================================================
                // 2. THERMODYNAMICS & DERIVED ENERGY VARIABLES
                // ====================================================================
                hr.Humidex = CalculateHumidex(temp, dewPoint);
                hr.AirDensity = CalculateAirDensity(temp, dewPoint, pressureHpa);
                hr.WindPowerDensity = CalculateWindPowerDensity(hr.AirDensity.Value, windSpeed100m);
                hr.PhotoThermalUnit = CalculateHourlyPTU(temp, hr.ShortwaveRadiation.Value);
            }

            // 1. Bulk insert the 10-year historical dataset
            await BulkInsertAsync(hourlyData, "HourlyReadings");

            // 2. Generate the TMY (Typical Meteorological Year) mapped to the year 2025
            var tmyData = GenerateTmyDataset(hourlyData, targetYear: 2025);

            // 3. Bulk insert the TMY dataset into its own table
            // Ensure you have created a "TmyReadings" table in your SQL database!
            await BulkInsertAsync(tmyData, "TmyReadings");
        }

        #region TMY Generation Engine

        private List<HourlyReading> GenerateTmyDataset(List<HourlyReading> historicalData, int targetYear)
        {
            var tmyData = new List<HourlyReading>();

            // Step A: Calculate Long-Term Averages (LTA) across all 10 years for each month
            // We focus on Temperature and Solar Radiation as they dictate thermodynamic limits
            var longTermAverages = historicalData
                .GroupBy(h => h.Time.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    AvgTemp = g.Average(x => x.Temperature2m ?? 0),
                    AvgGhi = g.Average(x => x.ShortwaveRadiation ?? 0)
                }).ToList();

            // Step B: Evaluate and stitch the "Most Typical" months
            for (int month = 1; month <= 12; month++)
            {
                var lta = longTermAverages.First(x => x.Month == month);

                // Calculate the specific averages for this month in every individual year
                var monthlyStatsByYear = historicalData
                    .Where(h => h.Time.Month == month)
                    .GroupBy(h => h.Time.Year)
                    .Select(g => new
                    {
                        Year = g.Key,
                        AvgTemp = g.Average(x => x.Temperature2m ?? 0),
                        AvgGhi = g.Average(x => x.ShortwaveRadiation ?? 0)
                    }).ToList();

                // Empirical Distance Formula: Find the year closest to the Long-Term Average
                // GHI is divided by 100 to normalize its weight against temperature degrees
                var bestYear = monthlyStatsByYear
                    .OrderBy(s => Math.Abs(s.AvgTemp - lta.AvgTemp) + (Math.Abs(s.AvgGhi - lta.AvgGhi) / 100.0))
                    .First().Year;

                // Extract all hourly readings for the winning Year/Month combination
                var typicalMonthReadings = historicalData
                    .Where(h => h.Time.Year == bestYear && h.Time.Month == month)
                    .Where(h => !(h.Time.Month == 2 && h.Time.Day == 29)) // Eradicate leap days to ensure a clean 365-day TMY
                    .ToList();

                // Step C: Map historical timestamps to the baseline target year
                foreach (var historicalReading in typicalMonthReadings)
                {
                    // Deep clone the object using JSON serialization to avoid mutating original memory references
                    var serializedString = JsonSerializer.Serialize(historicalReading);
                    var tmyReading = JsonSerializer.Deserialize<HourlyReading>(serializedString);

                    if (tmyReading != null)
                    {
                        // Overwrite the time to stitch it perfectly into the target year (e.g., 2025)
                        tmyReading.Time = new DateTime(targetYear, month, tmyReading.Time.Day, tmyReading.Time.Hour, tmyReading.Time.Minute, 0);

                        // Set the ID to 0 if it's an auto-incrementing primary key in EF Core
                        // tmyReading.Id = 0; 

                        tmyData.Add(tmyReading);
                    }
                }
            }

            return tmyData.OrderBy(x => x.Time).ToList();
        }

        #endregion

        #region Scientific Calculation Logic

        private double CalculateHumidex(double tempC, double dewPointC)
        {
            double e = 6.11 * Math.Pow(10.0, (7.5 * dewPointC) / (237.3 + dewPointC));
            double humidex = tempC + (5.0 / 9.0) * (e - 10.0);
            return Math.Round(humidex, 2);
        }

        private double CalculateAirDensity(double tempC, double dewPointC, double surfacePressureHpa)
        {
            double T_K = tempC + 273.15;
            double e = 6.11 * Math.Pow(10.0, (7.5 * dewPointC) / (237.3 + dewPointC));
            double p_v = e * 100.0;
            double p_total = surfacePressureHpa * 100.0;
            double p_d = p_total - p_v;

            double density = (p_d / (R_d * T_K)) + (p_v / (R_v * T_K));
            return Math.Round(density, 4);
        }

        private double CalculateWindPowerDensity(double density, double windSpeedKmh)
        {
            double velocityMs = windSpeedKmh / 3.6;
            double wpd = 0.5 * density * Math.Pow(velocityMs, 3);
            return Math.Round(wpd, 2);
        }

        private double CalculateHourlyPTU(double tempC, double ghi)
        {
            double hourlyGdd = Math.Max((tempC - T_base_Crop) / 24.0, 0);
            return Math.Round(hourlyGdd * ghi, 2);
        }

        #endregion

        #region File IO & Bulk Operations

        private List<T> ParseCsv<T>(string filePath, int skipLines)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                PrepareHeaderForMatch = args => args.Header.ToLower()
                    .Split(' ')[0]
                    .Replace("_", "")
                    .Replace("-", ""),
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(filePath);
            for (int i = 0; i < skipLines; i++) reader.ReadLine();

            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<T>().ToList();
        }

        private async Task BulkInsertAsync<T>(IEnumerable<T> data, string tableName)
        {
            using var table = new DataTable();
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id") // Ignore auto-incrementing ID
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

            foreach (var prop in properties)
                bulkCopy.ColumnMappings.Add(prop.Name, prop.Name);

            await bulkCopy.WriteToServerAsync(table);
        }

        #endregion
    }
}