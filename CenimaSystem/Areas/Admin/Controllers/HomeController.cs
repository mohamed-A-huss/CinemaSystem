using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController()
        {
            _context = new();
        }
        public IActionResult Index()
        {
            ViewBag.Movies = _context.Movies.Count();
            ViewBag.Categories = _context.Categories.Count();
            ViewBag.Actors = _context.Actors.Count();
            ViewBag.Cinemas = _context.Cinemas.Count();

            return View();
        }
    }
}
