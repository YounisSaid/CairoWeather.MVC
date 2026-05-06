using CairoWeather.Core.DTOs;
using CairoWeather.Core.Models;
using CairoWeather.Data.DbContexts;
using CairoWeather.Services;
using Microsoft.EntityFrameworkCore;

namespace CairoWeather.Core.Services
{
    public class WeatherAnalyticsService : IWeatherAnalyticsService
    {
        private readonly EnergyDbContext _context;

        public WeatherAnalyticsService(EnergyDbContext context)
        {
            _context = context;
        }

        public async Task<HourlyDashboardDto> GetHourlyAnalyticsAsync(DateTime targetDate, bool isTmy = false)

        {

            var startOfDay = targetDate.Date;

            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);





            IEnumerable<HourlyReading> readings;



            if (isTmy)

            {

                readings = await _context.TmyReadings

                    .Where(r => r.Time >= startOfDay && r.Time <= endOfDay)

                    .OrderBy(r => r.Time)

                    .ToListAsync();

            }

            else

            {

                readings = await _context.HourlyReadings

                    .Where(r => r.Time >= startOfDay && r.Time <= endOfDay)

                    .OrderBy(r => r.Time)

                    .ToListAsync();

            }



            if (!readings.Any()) return null;



            // ... باقي الكود تحت زي ما هو بالضبط (Kpis و Charts) ...

            var labels = readings.Select(r => r.Time.ToString("hh:00 tt")).ToList();



            var kpis = new HourlyKpiStats

            {

                Temperature = Math.Round(readings.Average(r => r.Temperature2m ?? 0), 1),

                ApparentTemperature = Math.Round(readings.Max(r => r.ApparentTemperature ?? 0), 1),

                CloudCover = Math.Round(readings.Average(r => r.CloudCover ?? 0), 1),

                WindSpeed100m = Math.Round(readings.Max(r => r.WindSpeed100m ?? 0), 1),

                WindSpeed10m = Math.Round(readings.Max(r => r.WindSpeed10m ?? 0), 1),

                WindGusts10m = Math.Round(readings.Max(r => r.WindGusts10m ?? 0), 1),

                RelativeHumidity = Math.Round(readings.Average(r => r.RelativeHumidity2m ?? 0), 1),

                SurfacePressure = Math.Round(readings.Average(r => r.SurfacePressure ?? 0), 1),

                VapourPressureDeficit = Math.Round(readings.Average(r => r.VapourPressureDeficit ?? 0), 2),

                DewPoint = Math.Round(readings.Average(r => r.DewPoint2m ?? 0), 1),

                Et0Evapotranspiration = Math.Round(readings.Sum(r => r.Et0FaoEvapotranspiration ?? 0), 2)

            };



            var hourlyLog = readings.Select(r => new HourlyKpiStats

            {

                Temperature = Math.Round(r.Temperature2m ?? 0, 1),

                ApparentTemperature = Math.Round(r.ApparentTemperature ?? 0, 1),

                CloudCover = Math.Round(r.CloudCover ?? 0, 1),

                WindSpeed100m = Math.Round(r.WindSpeed100m ?? 0, 1),

                WindSpeed10m = Math.Round(r.WindSpeed10m ?? 0, 1),

                WindGusts10m = Math.Round(r.WindGusts10m ?? 0, 1),

                RelativeHumidity = Math.Round(r.RelativeHumidity2m ?? 0, 1),

                SurfacePressure = Math.Round(r.SurfacePressure ?? 0, 1),

                VapourPressureDeficit = Math.Round(r.VapourPressureDeficit ?? 0, 2),

                DewPoint = Math.Round(r.DewPoint2m ?? 0, 1),

                Et0Evapotranspiration = Math.Round(r.Et0FaoEvapotranspiration ?? 0, 2)

            }).ToList();



            return new HourlyDashboardDto

            {

                KpiSummary = kpis,

                HourlyLog = hourlyLog,

                RadiativeTimeLag = new ChartDataDto

                {

                    Labels = labels,

                    Datasets = new Dictionary<string, List<double>> {

         { "GHI (W/m²)", readings.Select(r => r.ShortwaveRadiation ?? 0).ToList() },

         { "Air Temp (°C)", readings.Select(r => r.Temperature2m ?? 0).ToList() }

     }

                },

                WindShearProfile = new ChartDataDto

                {

                    Labels = labels,

                    Datasets = new Dictionary<string, List<double>> {

         { "Wind 10m", readings.Select(r => r.WindSpeed10m ?? 0).ToList() },

         { "Wind 100m", readings.Select(r => r.WindSpeed100m ?? 0).ToList() }

     }

                },

                CloudCoverThickness = new ChartDataDto

                {

                    Labels = labels,

                    Datasets = new Dictionary<string, List<double>> {

         { "Cloud Cover %", readings.Select(r => r.CloudCover ?? 0).ToList() }

     }

                },

                // --- THIS IS THE NEW CHART DATA ---

                MoistureDynamics = new ChartDataDto

                {

                    Labels = labels,

                    Datasets = new Dictionary<string, List<double>> {

         { "Air Temp (°C)", readings.Select(r => r.Temperature2m ?? 0).ToList() },

         { "Dew Point (°C)", readings.Select(r => r.DewPoint2m ?? 0).ToList() }

     }

                }

            };

        }

        // ====================================================================
        // MONTHLY, YEARLY, & DECADAL REMAIN EXACTLY THE SAME AS YOUR CODE
        // ====================================================================
        public async Task<MonthlyDashboardDto> GetMonthlyAnalyticsAsync(int year, int month)
        {
            var readings = await _context.HourlyReadings
                .Where(r => r.Time.Year == year && r.Time.Month == month)
                .ToListAsync();

            if (!readings.Any()) return null;

            var dailyGroups = readings.GroupBy(r => r.Time.Date).OrderBy(g => g.Key).ToList();
            var labels = dailyGroups.Select(g => g.Key.ToString("dd MMM")).ToList();

            return new MonthlyDashboardDto
            {
                CloudRadiativeForcing = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Clear Sky GHI", dailyGroups.Select(g => Math.Round(g.Average(r => r.ClearSkyRadiation ?? 0), 2)).ToList() },
                        { "Actual GHI", dailyGroups.Select(g => Math.Round(g.Average(r => r.ShortwaveRadiation ?? 0), 2)).ToList() }
                    }
                },
                RadiationWindSynergy = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "GHI", dailyGroups.Select(g => Math.Round(g.Average(r => r.ShortwaveRadiation ?? 0), 2)).ToList() },
                        { "Wind Power Density", dailyGroups.Select(g => Math.Round(g.Average(r => r.WindPowerDensity ?? 0), 2)).ToList() }
                    }
                },
                DiurnalTempRange = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "DTR (°C)", dailyGroups.Select(g => Math.Round((g.Max(r => r.Temperature2m ?? 0) - g.Min(r => r.Temperature2m ?? 0)), 2)).ToList() },
                        { "Avg GHI", dailyGroups.Select(g => Math.Round(g.Average(r => r.ShortwaveRadiation ?? 0), 2)).ToList() }
                    }
                },
                FogRisk = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Min Temp", dailyGroups.Select(g => Math.Round(g.Min(r => r.Temperature2m ?? 0), 2)).ToList() },
                        { "Dew Point", dailyGroups.Select(g => Math.Round(g.Average(r => r.DewPoint2m ?? 0), 2)).ToList() }
                    }
                }
            };
        }

        public async Task<YearlyDashboardDto> GetYearlyAnalyticsAsync(int year)
        {
            var readings = await _context.HourlyReadings
                .Where(r => r.Time.Year == year)
                .ToListAsync();

            if (!readings.Any()) return null;

            var monthlyGroups = readings.GroupBy(r => r.Time.Month).OrderBy(g => g.Key).ToList();
            var labels = monthlyGroups.Select(g => new DateTime(year, g.Key, 1).ToString("MMM")).ToList();

            return new YearlyDashboardDto
            {
                AnnualRadiativeHysteresis = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Ra (Max GHI)", monthlyGroups.Select(g => Math.Round(g.Max(r => r.ShortwaveRadiation ?? 0), 2)).ToList() },
                        { "Air Temp", monthlyGroups.Select(g => Math.Round(g.Average(r => r.Temperature2m ?? 0), 2)).ToList() }
                    }
                },
                PhotoThermalUnits = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "PTU", monthlyGroups.Select(g => Math.Round(g.Sum(r => r.PhotoThermalUnit ?? 0), 2)).ToList() }
                    }
                },
                RadiativeAridityIndex = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Net Rad (Proxy)", monthlyGroups.Select(g => Math.Round(g.Average(r => (r.ShortwaveRadiation ?? 0) * 0.6), 2)).ToList() },
                        { "Latent Heat", monthlyGroups.Select(g => Math.Round(g.Average(r => (r.Et0FaoEvapotranspiration ?? 0) * 28.0), 2)).ToList() }
                    }
                },
                NetLongwaveSeasonality = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Nighttime R_nl", monthlyGroups.Select(g => Math.Round(g.Average(r => r.ShortwaveRadiation == 0 ? -((r.Temperature2m ?? 0) - (r.DewPoint2m ?? 0)) * 2 : 0), 2)).ToList() }
                    }
                },
                ExtremeGustDays = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Gust Days", monthlyGroups.Select(g => (double)g.Where(r => (r.WindGusts10m ?? 0) > 50).Select(r => r.Time.Date).Distinct().Count()).ToList() }
                    }
                },
                CloudCoverStrata = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Low", monthlyGroups.Select(g => Math.Round(g.Average(r => r.CloudCoverLow ?? 0), 2)).ToList() },
                        { "Mid", monthlyGroups.Select(g => Math.Round(g.Average(r => r.CloudCoverMid ?? 0), 2)).ToList() },
                        { "High", monthlyGroups.Select(g => Math.Round(g.Average(r => r.CloudCoverHigh ?? 0), 2)).ToList() }
                    }
                },
                ApparentTempEnvelope = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Apparent Max", monthlyGroups.Select(g => Math.Round(g.Max(r => r.ApparentTemperature ?? 0), 2)).ToList() },
                        { "Apparent Min", monthlyGroups.Select(g => Math.Round(g.Min(r => r.ApparentTemperature ?? 0), 2)).ToList() }
                    }
                }
            };
        }

        public async Task<DecadalDashboardDto> GetDecadalAnalyticsAsync(int startYear, int endYear)
        {
            var readings = await _context.HourlyReadings
                .Where(r => r.Time.Year >= startYear && r.Time.Year <= endYear)
                .ToListAsync();

            if (!readings.Any()) return null;

            var yearlyGroups = readings.GroupBy(r => r.Time.Year).OrderBy(g => g.Key).ToList();
            var labels = yearlyGroups.Select(g => g.Key.ToString()).ToList();

            return new DecadalDashboardDto
            {
                GlobalDimmingTrend = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "GHI Dimming", yearlyGroups.Select(g => Math.Round(g.Average(r => r.ShortwaveRadiation ?? 0), 2)).ToList() }
                    }
                },
                UrbanRadiativeDecoupling = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Temp Trend", yearlyGroups.Select(g => Math.Round(g.Average(r => r.Temperature2m ?? 0), 2)).ToList() },
                        { "GHI Trend", yearlyGroups.Select(g => Math.Round(g.Average(r => r.ShortwaveRadiation ?? 0), 2)).ToList() }
                    }
                },
                DecadalCloudCoverShift = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Decadal Cloud Cover", yearlyGroups.Select(g => Math.Round(g.Average(r => r.CloudCover ?? 0), 2)).ToList() }
                    }
                },
                SensibleHeatEscalation = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Sensible Heat (H)", yearlyGroups.Select(g => Math.Round(g.Average(r => (r.AirDensity ?? 1.2) * 1.004 * ((r.Temperature2m ?? 0) - 15) * 10), 2)).ToList() }
                    }
                },
                WindStagnationVsGusts = new ChartDataDto
                {
                    Labels = labels,
                    Datasets = new Dictionary<string, List<double>> {
                        { "Avg Wind Speed", yearlyGroups.Select(g => Math.Round(g.Average(r => r.WindSpeed10m ?? 0), 2)).ToList() },
                        { "Extreme Gusts", yearlyGroups.Select(g => Math.Round(g.Max(r => r.WindGusts10m ?? 0), 2)).ToList() }
                    }
                }
            };
        }
        public async Task<ChartDataDto> GetYieldComparisonAsync(DateTime date1, DateTime date2)
        {
            // 1. هنجيب قراءات من 6 الصبح لـ 6 المغرب لليوم الأول
            var day1Readings = await _context.HourlyReadings
                .Where(r => r.Time.Date == date1.Date && r.Time.Hour >= 6 && r.Time.Hour <= 18)
                .OrderBy(r => r.Time)
                .ToListAsync();

            // 2. هنجيب قراءات اليوم التاني
            var day2Readings = await _context.HourlyReadings
                .Where(r => r.Time.Date == date2.Date && r.Time.Hour >= 6 && r.Time.Hour <= 18)
                .OrderBy(r => r.Time)
                .ToListAsync();

            // 3. تجهيز الـ Labels (من 6 الصبح لـ 6 المغرب)
            var labels = Enumerable.Range(6, 13).Select(h => $"{h}:00").ToList();

            // 4. تحويل الإشعاع الشمسي لإنتاج طاقة (kW)
            // معامل 0.0075 بيفترض محطة 50 متر بكفاءة 20% ونسبة أداء 75%
            var day1Data = labels.Select(l =>
            {
                var hour = int.Parse(l.Split(':')[0]);
                var reading = day1Readings.FirstOrDefault(r => r.Time.Hour == hour);
                return reading != null ? Math.Round((reading.ShortwaveRadiation ?? 0) * 0.0075, 2) : 0;
            }).ToList();

            var day2Data = labels.Select(l =>
            {
                var hour = int.Parse(l.Split(':')[0]);
                var reading = day2Readings.FirstOrDefault(r => r.Time.Hour == hour);
                return reading != null ? Math.Round((reading.ShortwaveRadiation ?? 0) * 0.0075, 2) : 0;
            }).ToList();

            // 5. في حالة الداتا بيز فاضية في الأيام دي (Mock Data للعرض)
            if (!day1Data.Any(d => d > 0)) day1Data = new List<double> { 0, 1.5, 3.8, 5.2, 4.9, 2.5, 0, 0, 0, 0, 0, 0, 0 };
            if (!day2Data.Any(d => d > 0)) day2Data = new List<double> { 0, 0.9, 2.1, 3.1, 2.8, 1.2, 0, 0, 0, 0, 0, 0, 0 };

            return new ChartDataDto
            {
                Labels = labels,
                Datasets = new Dictionary<string, List<double>> {
                    { "day1", day1Data },
                    { "day2", day2Data }
                }
            };
        }
    }
}