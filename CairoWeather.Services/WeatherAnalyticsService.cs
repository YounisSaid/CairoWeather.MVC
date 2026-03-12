using CairoWeather.Core.DTOs;
using CairoWeather.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CairoWeather.Services
{
    public interface IWeatherAnalyticsService
    {
        Task<HourlyAnalyticsDto> GetHourlyAnalyticsAsync(DateTime date);
        Task<Dictionary<string, MetricChartDto>> GetYearlyAnalyticsAsync(int year);
        Task<Dictionary<string, MetricChartDto>> GetDecadeAnalyticsAsync();
    }

    public class WeatherAnalyticsService : IWeatherAnalyticsService
    {
        private readonly EnergyDbContext _context;

        public WeatherAnalyticsService(EnergyDbContext context)
        {
            _context = context;
        }

        // --- SECTION 1: HOURLY (Unchanged) ---
        public async Task<HourlyAnalyticsDto> GetHourlyAnalyticsAsync(DateTime date)
        {
            var readings = await _context.HourlyReadings.Where(h => h.Time.Date == date.Date).OrderBy(h => h.Time).ToListAsync();
            if (!readings.Any()) return null;

            var dto = new HourlyAnalyticsDto { Labels = readings.Select(r => r.Time.ToString("HH:00")).ToList() };
            dto.Datasets["wind100m"] = readings.Select(r => r.WindSpeed100m).ToList();
            dto.Datasets["wind10m"] = readings.Select(r => r.WindSpeed10m).ToList();
            dto.Datasets["gusts"] = readings.Select(r => r.WindGusts10m).ToList();
            dto.Datasets["temp"] = readings.Select(r => r.Temperature2m).ToList();
            dto.Datasets["appTemp"] = readings.Select(r => r.ApparentTemperature).ToList();
            dto.Datasets["dewPoint"] = readings.Select(r => r.DewPoint2m).ToList();
            dto.Datasets["vpd"] = readings.Select(r => r.VapourPressureDeficit).ToList();
            dto.Datasets["et0"] = readings.Select(r => r.Et0FaoEvapotranspiration).ToList();
            dto.Datasets["soilT7"] = readings.Select(r => r.SoilTemperature0To7cm).ToList();
            dto.Datasets["soilT28"] = readings.Select(r => r.SoilTemperature7To28cm).ToList();
            dto.Datasets["soilT100"] = readings.Select(r => r.SoilTemperature28To100cm).ToList();
            dto.Datasets["soilT255"] = readings.Select(r => r.SoilTemperature100To255cm).ToList();
            dto.Datasets["cloudLow"] = readings.Select(r => r.CloudCoverLow).ToList();
            dto.Datasets["cloudMid"] = readings.Select(r => r.CloudCoverMid).ToList();
            dto.Datasets["cloudHigh"] = readings.Select(r => r.CloudCoverHigh).ToList();
            dto.Datasets["cloudTotal"] = readings.Select(r => r.CloudCover).ToList();
            dto.Datasets["soilM7"] = readings.Select(r => r.SoilMoisture0To7cm).ToList();
            dto.Datasets["soilM28"] = readings.Select(r => r.SoilMoisture7To28cm).ToList();
            dto.Datasets["soilM100"] = readings.Select(r => r.SoilMoisture28To100cm).ToList();
            dto.Datasets["soilM255"] = readings.Select(r => r.SoilMoisture100To255cm).ToList();
            dto.Datasets["pressSfc"] = readings.Select(r => r.SurfacePressure).ToList();
            dto.Datasets["pressMsl"] = readings.Select(r => r.PressureMsl).ToList();
            dto.Datasets["rh"] = readings.Select(r => r.RelativeHumidity2m).ToList();

            var midday = readings.ElementAtOrDefault(12) ?? readings.First();
            dto.Summary = new WeatherStats
            {
                Temp = Math.Round(midday.Temperature2m, 1),
                ApparentTemp = Math.Round(midday.ApparentTemperature, 1),
                Humidity = Math.Round(midday.RelativeHumidity2m, 1),
                WindSpeed = Math.Round(midday.WindSpeed100m, 1),
                WindGust = Math.Round(midday.WindGusts10m, 1),
                Et0 = Math.Round(midday.Et0FaoEvapotranspiration, 2),
                Vpd = Math.Round(midday.VapourPressureDeficit, 2),
                Pressure = Math.Round(midday.SurfacePressure, 1),
                CloudCover = Math.Round(midday.CloudCover, 1),
                DewPoint = Math.Round(midday.DewPoint2m, 1)
            };
            return dto;
        }

        // --- SECTION 2: YEARLY (Unchanged) ---
        public async Task<Dictionary<string, MetricChartDto>> GetYearlyAnalyticsAsync(int year)
        {
            var data = await _context.DailyReadings.Where(d => d.Date.Year == year).OrderBy(d => d.Date).ToListAsync();
            if (!data.Any()) return null;

            var monthly = data.GroupBy(d => d.Date.Month).Select(g => new
            {
                Month = g.Key,
                AvgRad = g.Average(x => x.ShortwaveRadiationSum),
                SunshineHrs = g.Average(x => x.SunshineDuration) / 3600.0,
                MaxTemp = g.Average(x => x.Temperature2mMax),
                MinTemp = g.Average(x => x.Temperature2mMin),
                MeanTemp = g.Average(x => x.Temperature2mMean),
                MaxWind = g.Average(x => x.WindSpeed10mMax),
                MaxGust = g.Average(x => x.WindGusts10mMax),
                CloudMean = g.Average(x => x.CloudCoverMean),
                SoilTemp = g.Average(x => x.SoilTemperature0To100cmMean),
                Vpd = g.Average(x => x.VapourPressureDeficitMax),
                Et0 = g.Average(x => x.Et0FaoEvapotranspirationSum)
            }).OrderBy(x => x.Month).ToList();

            var months = monthly.Select(m => System.Globalization.DateTimeFormatInfo.InvariantInfo.GetAbbreviatedMonthName(m.Month)).ToList();

            return new Dictionary<string, MetricChartDto>
            {
                ["solar"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.AvgRad, 2)).ToList() },
                ["sunshine"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.SunshineHrs, 1)).ToList() },
                ["tempMax"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.MaxTemp, 1)).ToList() },
                ["tempMean"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.MeanTemp, 1)).ToList() },
                ["tempMin"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.MinTemp, 1)).ToList() },
                ["wind"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.MaxWind, 1)).ToList() },
                ["gust"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.MaxGust, 1)).ToList() },
                ["cloud"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.CloudMean, 1)).ToList() },
                ["soil"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.SoilTemp, 1)).ToList() },
                ["vpd"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.Vpd, 2)).ToList() },
                ["et0"] = new MetricChartDto { Labels = months, Values = monthly.Select(m => Math.Round(m.Et0, 2)).ToList() }
            };
        }

        // --- SECTION 3: DECADE (MASSIVELY EXPANDED) ---
        public async Task<Dictionary<string, MetricChartDto>> GetDecadeAnalyticsAsync()
        {
            var data = await _context.DailyReadings.OrderBy(d => d.Date).ToListAsync();
            if (!data.Any()) return null;

            var annual = data.GroupBy(d => d.Date.Year).Select(g => new
            {
                Year = g.Key,
                AvgRad = g.Average(x => x.ShortwaveRadiationSum),
                AvgCloud = g.Average(x => x.CloudCoverMean),
                PeakGust = g.Max(x => x.WindGusts10mMax), // Absolute worst gust of the year
                AvgWind = g.Average(x => x.WindSpeed10mMean),
                HeatDays35 = g.Count(x => x.Temperature2mMax >= 35), // Thermal throttling limit
                HeatDays40 = g.Count(x => x.Temperature2mMax >= 40), // Extreme danger limit
                TotalEt0 = g.Sum(x => x.Et0FaoEvapotranspirationSum), // Total annual evaporation
                // What % of daylight was actually sunny?
                SunshineRatio = (g.Sum(x => x.SunshineDuration) / (g.Sum(x => x.DaylightDuration) + 1)) * 100,
                AvgWetBulb = g.Average(x => x.WetBulbTemperature2mMax),
                AvgSoilTemp = g.Average(x => x.SoilTemperature0To100cmMean),
                AvgSoilMoist = g.Average(x => x.SoilMoisture28To100cmMean),
                AvgHumMax = g.Average(x => x.RelativeHumidity2mMax),
                AvgHumMin = g.Average(x => x.RelativeHumidity2mMin)
            }).OrderBy(x => x.Year).ToList();

            var years = annual.Select(a => a.Year.ToString()).ToList();

            return new Dictionary<string, MetricChartDto>
            {
                ["radDecade"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.AvgRad, 2)).ToList() },
                ["cloudDecade"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.AvgCloud, 1)).ToList() },
                ["gustDecade"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.PeakGust, 1)).ToList() },
                ["windDecade"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.AvgWind, 1)).ToList() },
                ["heat35"] = new MetricChartDto { Labels = years, Values = annual.Select(a => (double)a.HeatDays35).ToList() },
                ["heat40"] = new MetricChartDto { Labels = years, Values = annual.Select(a => (double)a.HeatDays40).ToList() },
                ["et0Total"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.TotalEt0, 0)).ToList() },
                ["sunRatio"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.SunshineRatio, 1)).ToList() },
                ["wetBulb"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.AvgWetBulb, 1)).ToList() },
                ["soilTempDecade"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.AvgSoilTemp, 1)).ToList() },
                ["soilMoistDecade"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.AvgSoilMoist, 3)).ToList() },
                ["humMax"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.AvgHumMax, 1)).ToList() },
                ["humMin"] = new MetricChartDto { Labels = years, Values = annual.Select(a => Math.Round(a.AvgHumMin, 1)).ToList() }
            };
        }
    }
}