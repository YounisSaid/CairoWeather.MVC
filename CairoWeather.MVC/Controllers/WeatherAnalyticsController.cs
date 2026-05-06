using CairoWeather.Core.DTOs;
// Ensure this using statement points to where your interface is located
using CairoWeather.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoWeather.MVC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherAnalyticsController : ControllerBase
    {
        // 1. FIXED: Changed to use the Interface (IWeatherAnalyticsService)
        private readonly IWeatherAnalyticsService _analyticsService;

        // 2. FIXED: Changed the constructor parameter to use the Interface
        public WeatherAnalyticsController(IWeatherAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("hourly")]
        public async Task<ActionResult<HourlyDashboardDto>> GetHourlyDynamics([FromQuery] string date, [FromQuery] bool isTmy = false)
        {
            if (!DateTime.TryParse(date, out DateTime targetDate))
                return BadRequest(new { message = "Invalid date format. Please use YYYY-MM-DD." });

            var data = await _analyticsService.GetHourlyAnalyticsAsync(targetDate, isTmy);

            if (data == null)
                return NotFound(new { message = "No data available for this date in the selected table." });

            return Ok(data);
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<MonthlyDashboardDto>> GetMonthlyAggregation([FromQuery] int year, [FromQuery] int month)
        {
            var data = await _analyticsService.GetMonthlyAnalyticsAsync(year, month);

            if (data == null)
                return NotFound("No monthly data available for this timeframe.");

            return Ok(data);
        }

        [HttpGet("yearly")]
        public async Task<ActionResult<YearlyDashboardDto>> GetYearlySeasonality([FromQuery] int year)
        {
            var data = await _analyticsService.GetYearlyAnalyticsAsync(year);

            if (data == null)
                return NotFound("No yearly data available for this timeframe.");

            return Ok(data);
        }

        [HttpGet("decadal")]
        public async Task<ActionResult<DecadalDashboardDto>> GetDecadalTrends([FromQuery] int startYear, [FromQuery] int endYear)
        {
            var data = await _analyticsService.GetDecadalAnalyticsAsync(startYear, endYear);

            if (data == null)
                return NotFound("No decadal data available for this timeframe.");

            return Ok(data);
        }
    }
}