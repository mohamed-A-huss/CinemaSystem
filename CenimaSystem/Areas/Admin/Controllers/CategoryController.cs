using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController()
        {
            _context = new();
        }
        public IActionResult Index(int page = 1, string? query = null)
        {

            var categories = _context.Categories.AsQueryable();
            if (query is not null)
            {
                categories = categories.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));
                ViewBag.Query = query;
                
            }

            // Pagination
            double totalPages = Math.Ceiling(categories.Count() / 3.0);
            categories = categories.Skip((page - 1) * 3).Take(3);
            return View(new CategoriesVM()
            {
                Categories = categories.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _context.Categories.Update(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

    }
}
