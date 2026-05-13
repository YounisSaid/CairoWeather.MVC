using CairoWeather.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoWeather.MVC.Controllers
{
    // The 'partial' keyword allows us to split the views across multiple files
    public partial class DashboardController : Controller
    {
        private readonly IWeatherAnalyticsService _analyticsService;

        public DashboardController(IWeatherAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
    }
}