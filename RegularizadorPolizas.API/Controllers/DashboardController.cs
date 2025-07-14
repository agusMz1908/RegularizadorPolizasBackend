using Microsoft.AspNetCore.Mvc;

namespace RegularizadorPolizas.API.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
