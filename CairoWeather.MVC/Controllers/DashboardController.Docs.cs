using Microsoft.AspNetCore.Mvc;

namespace CairoWeather.MVC.Controllers
{
    public partial class DashboardController
    {
        // GET: /Dashboard/Formulas
        public IActionResult Formulas() { return View(); }

        // GET: /Dashboard/DataSources
        public IActionResult DataSources() { return View(); }

        // GET: /Dashboard/OurTeam
        public IActionResult OurTeam() { return View(); }
    }
}