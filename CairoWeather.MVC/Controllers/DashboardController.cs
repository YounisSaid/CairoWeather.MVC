using CairoWeather.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoWeather.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IWeatherAnalyticsService _analyticsService;

        public DashboardController(IWeatherAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // Renders the main Dashboard View (Index.cshtml)
        public IActionResult Index()
        {
            return View();
        }

        // Endpoint for Section 1: Hourly Data
        [HttpGet]
        public async Task<JsonResult> GetHourlyData(string date)
        {
            // Default fallback to ensure the charts load if the date is empty or invalid
            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                parsedDate = new DateTime(2020, 3, 13);
            }

            var result = await _analyticsService.GetHourlyAnalyticsAsync(parsedDate);
            return Json(result);
        }

        // Endpoint for Section 2: Yearly Data
        [HttpGet]
        public async Task<JsonResult> GetYearlyData(int year)
        {
            // If year is invalid (e.g., 0), default to 2020
            if (year <= 0) year = 2020;

            var result = await _analyticsService.GetYearlyAnalyticsAsync(year);
            return Json(result);
        }

        // Endpoint for Section 3: Decade Data
        [HttpGet]
        public async Task<JsonResult> GetDecadeData()
        {
            var result = await _analyticsService.GetDecadeAnalyticsAsync();
            return Json(result);
        }
    }
}