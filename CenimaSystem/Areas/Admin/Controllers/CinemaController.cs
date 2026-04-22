using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]

    public class CinemaController : Controller
    {
        private readonly IRepository<Cinema> _repository;// = new();

        public CinemaController(IRepository<Cinema> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            var cinemas = await _repository.GetAsync(cancellationToken: cancellationToken);
            if (query is not null)
            {
                cinemas = cinemas.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));

            }

            // Pagination
            double totalPages = Math.Ceiling(cinemas.Count() / 3.0);
            cinemas = cinemas.Skip((page - 1) * 3).Take(3);
            return View(new CinemasVM()
            {
                Cinemas = cinemas.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query= query
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Cinema());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema,IFormFile Img, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }
            if (Img is not null && Img.Length > 0)
            {
                string fileName = CreateFile(Img);

                cinema.Img = fileName;
            }
             await _repository.CreateAsync(cinema, cancellationToken);
            await _repository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Cinema Added successfully!";


            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]

        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            var cinema =await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (cinema is null)
                return NotFound();

            return View(cinema);
        }
        [HttpPost]

        public async Task<IActionResult> Update(Cinema cinema, IFormFile Img, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }
            var cinemaFromDb = await _repository.GetOneAsync(e => e.Id == cinema.Id, cancellationToken: cancellationToken,tracked:false);

            if (Img is not null && Img.Length > 0)
            {
                string fileName = CreateFile(Img);
                var oldFilePath = GetOldFilePath(cinemaFromDb.Img);
                if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                cinema.Img = fileName;

            }
            else
            {
                cinema.Img = cinemaFromDb.Img;
            }

             _repository.Update(cinema);
            await _repository.CommitAsync(cancellationToken);
     
            TempData["success-notification"] = "Cinema Updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)

        {
            var cinema =await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (cinema is null)
                return NotFound();

            var oldFilePath = GetOldFilePath(cinema.Img);
            if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            _repository.Delete(cinema);
            await _repository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Cinema Deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
        private string CreateFile(IFormFile Img)
        {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + Guid.NewGuid().ToString()
                    + Path.GetExtension(Img.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinema", fileName);
            using (var stram = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stram);
            }
            return fileName;
        }
        private string GetOldFilePath(string oldFileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cinema", oldFileName);
            return filePath;
        }
    }
}
