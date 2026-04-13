
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _repository;// = new();

        public CategoryController(IRepository<Category> repository)
        {
            //_context = new();
            _repository = repository;
        }
        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {

            var categories = await _repository.GetAsync(cancellationToken: cancellationToken);
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
            return View(new Category());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return View(category);

            await _repository.CreateAsync(category, cancellationToken);
            await _repository.CommitAsync(cancellationToken);
            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            var category =await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (category == null) return NotFound();

            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return View(category);

            _repository.Update(category);
            await _repository.CommitAsync(cancellationToken);
            TempData["Success"] = "Category Updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var category = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (category == null) return NotFound();

            _repository.Delete(category);
            await _repository.CommitAsync(cancellationToken);
            TempData["Success"] = "Category Deleted successfully!";

            return RedirectToAction(nameof(Index));
        }

    }
}
