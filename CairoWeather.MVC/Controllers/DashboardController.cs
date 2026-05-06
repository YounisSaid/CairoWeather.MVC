using CairoWeather.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoWeather.MVC.Controllers
{
    public class DashboardController : Controller
    {

        private readonly IWeatherAnalyticsService _analyticsService;

        public DashboardController(IWeatherAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }


        // GET: /Dashboard/Hourly
        public IActionResult Hourly()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Tmy()
        {
            return View();
        }
        // GET: /Dashboard/Monthly
        public IActionResult Monthly()
        {
            return View();
        }

        // GET: /Dashboard/Yearly
        public IActionResult Yearly()
        {
            return View();
        }

        // GET: /Dashboard/Decadal
        public IActionResult Decadal()
        {
            return View();
        }

        // GET: /Dashboard/Solar
        public IActionResult SolarSimulator() { return View(); }
        public IActionResult SolarInverter() { return View(); }
        public IActionResult SolarSoiling() { return View(); }
        public IActionResult SolarShading() { return View(); }
        public IActionResult SolarBifacial() { return View(); }
        public IActionResult SolarTracking() { return View(); }
        public IActionResult SolarPlanner() { return View(); }

        public IActionResult InteractiveSimulators()
        {
            return View();
        }
        public IActionResult Formulas()
        {
            return View();
        }
        public IActionResult DataSources()
        {
            return View();
        }
        [HttpGet]
        [Route("api/Dashboard/InteractiveSimulators")]
        public async Task<IActionResult> CompareDays(DateTime date1, DateTime date2)
        {
            try
            {
                // نكلم الـ Service ترجع الداتا
                var chartData = await _analyticsService.GetYieldComparisonAsync(date1, date2);

                // نرجعها بصيغة JSON زي ما الـ JavaScript متوقع
                return Json(new
                {
                    labels = chartData.Labels,
                    day1 = chartData.Datasets["day1"],
                    day2 = chartData.Datasets["day2"]
                });
            }
            catch (Exception ex)
            {
                // Fallback in case of error
                return Json(new
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
