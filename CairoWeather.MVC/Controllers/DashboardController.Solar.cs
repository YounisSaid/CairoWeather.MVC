using Microsoft.AspNetCore.Mvc;

namespace CairoWeather.MVC.Controllers
{
    public partial class DashboardController
    {
        // GET: /Dashboard/StationConfigurator
        public IActionResult StationConfigurator() { return View(); }

        // GET: /Dashboard/SolarSimulator
        public IActionResult SolarSimulator() { return View(); }

        // GET: /Dashboard/SolarInverter
        public IActionResult SolarInverter() { return View(); }

        // GET: /Dashboard/SolarSoiling
        public IActionResult SolarSoiling() { return View(); }

        // GET: /Dashboard/SolarShading
        public IActionResult SolarShading() { return View(); }

        // GET: /Dashboard/SolarBifacial
        public IActionResult SolarBifacial() { return View(); }

        // GET: /Dashboard/SolarTracking
        public IActionResult SolarTracking() { return View(); }

        // GET: /Dashboard/SolarPlanner
        public IActionResult SolarPlanner() { return View(); }
    }
}