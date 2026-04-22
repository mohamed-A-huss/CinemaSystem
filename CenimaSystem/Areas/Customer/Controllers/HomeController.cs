using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        [Area(SD.CUSTOMER_AREA)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
