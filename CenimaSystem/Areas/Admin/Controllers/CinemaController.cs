using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]

    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CinemaController()
        {
            _context = new();
        }
        public IActionResult Index(int page = 1, string? query = null)
        {
            var cinemas = _context.Cinemas.AsQueryable();
            if (query is not null)
            {
                cinemas = cinemas.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));
                ViewBag.Query = query;

            }

            // Pagination
            double totalPages = Math.Ceiling(cinemas.Count() / 3.0);
            cinemas = cinemas.Skip((page - 1) * 3).Take(3);
            return View(new CinemasVM()
            {
                Cinemas = cinemas.AsEnumerable(),
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
        public IActionResult Create(Cinema cinema,IFormFile Img)
        {
            if (Img is not null && Img.Length > 0)
            {
                string fileName = CreateFile(Img);

                cinema.Img = fileName;
            }
            _context.Cinemas.Add(cinema);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]

        public IActionResult Update(int id)
        {
            var brand = _context.Cinemas.Find(id);
            if (brand is null)
                return NotFound();

            return View(brand);
        }
        [HttpPost]

        public IActionResult Update(Cinema cinema, IFormFile Img)
        {
            var cinemaFromDb = _context.Cinemas.AsNoTracking().FirstOrDefault(e => e.Id == cinema.Id);
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

            _context.Cinemas.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)

        {
            var cinema = _context.Cinemas.Find(id);

            if (cinema is null)
                return NotFound();

            var oldFilePath = GetOldFilePath(cinema.Img);
            if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
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
