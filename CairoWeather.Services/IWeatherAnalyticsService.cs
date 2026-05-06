using CairoWeather.Core.DTOs;

namespace CairoWeather.Services
{
    public interface IWeatherAnalyticsService
    {
        Task<HourlyDashboardDto> GetHourlyAnalyticsAsync(DateTime targetDate, bool isTmy = false);
        Task<MonthlyDashboardDto> GetMonthlyAnalyticsAsync(int year, int month);
        Task<YearlyDashboardDto> GetYearlyAnalyticsAsync(int year);
        Task<DecadalDashboardDto> GetDecadalAnalyticsAsync(int startYear, int endYear);
        Task<ChartDataDto> GetYieldComparisonAsync(DateTime date1, DateTime date2);
    }
}