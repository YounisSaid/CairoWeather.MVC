using CairoWeather.Core.DTOs;
using CairoWeather.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoWeather.MVC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherAnalyticsController : ControllerBase
    {
        private readonly IWeatherAnalyticsService _analyticsService;

        public WeatherAnalyticsController(IWeatherAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("hourly")]
        public async Task<ActionResult<HourlyDashboardDto>> GetHourlyDynamics([FromQuery] string date, [FromQuery] bool isTmy = false)
        {
            if (!DateTime.TryParse(date, out DateTime targetDate)) return BadRequest(new { message = "Invalid date format." });
            var data = await _analyticsService.GetHourlyAnalyticsAsync(targetDate, isTmy);
            if (data == null) return NotFound(new { message = "No data available." });
            return Ok(data);
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<MonthlyDashboardDto>> GetMonthlyAggregation([FromQuery] int year, [FromQuery] int month)
        {
            var data = await _analyticsService.GetMonthlyAnalyticsAsync(year, month);
            return data == null ? NotFound() : Ok(data);
        }

        [HttpGet("yearly")]
        public async Task<ActionResult<YearlyDashboardDto>> GetYearlySeasonality([FromQuery] int year)
        {
            var data = await _analyticsService.GetYearlyAnalyticsAsync(year);
            return data == null ? NotFound() : Ok(data);
        }

        [HttpGet("decadal")]
        public async Task<ActionResult<DecadalDashboardDto>> GetDecadalTrends([FromQuery] int startYear, [FromQuery] int endYear)
        {
            var data = await _analyticsService.GetDecadalAnalyticsAsync(startYear, endYear);
            return data == null ? NotFound() : Ok(data);
        }

        // --- MOVED FROM DASHBOARD CONTROLLER ---
        [HttpGet("/api/Dashboard/InteractiveSimulators")]
        public async Task<IActionResult> CompareDays(DateTime date1, DateTime date2)
        {
            try
            {
                var chartData = await _analyticsService.GetYieldComparisonAsync(date1, date2);
                return Ok(new
                {
                    labels = chartData.Labels,
                    day1 = chartData.Datasets["day1"],
                    day2 = chartData.Datasets["day2"]
                });
            }
            catch (Exception ex)
            {
                // Fallback in case of error
                return Ok(new
                {
                    labels = new List<string> { "6:00", "8:00", "10:00", "12:00", "14:00", "16:00", "18:00" },
                    day1 = new List<double> { 0, 1.5, 3.8, 5.2, 4.9, 2.5, 0 },
                    day2 = new List<double> { 0, 0.9, 2.1, 3.1, 2.8, 1.2, 0 },
                    error = ex.Message
                });
            }
        }
    }
}