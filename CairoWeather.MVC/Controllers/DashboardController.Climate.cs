using Microsoft.AspNetCore.Mvc;

namespace CairoWeather.MVC.Controllers
{
    public partial class DashboardController
    {
        // GET: /Dashboard/Hourly
        public IActionResult Hourly() { return View(); }

        // GET: /Dashboard/Tmy
        public IActionResult Tmy() { return View(); }

        // GET: /Dashboard/Monthly
        public IActionResult Monthly() { return View(); }

        // GET: /Dashboard/Yearly
        public IActionResult Yearly() { return View(); }

        // GET: /Dashboard/Decadal
        public IActionResult Decadal() { return View(); }

        // GET: /Dashboard/UhiHeatmap
        public IActionResult UhiHeatmap() { return View(); }

        // GET: /Dashboard/YieldComparison
        public IActionResult YieldComparison() { return View(); }
    }
}