using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ActorController()
        {
            _context = new();
        }
        public IActionResult Index(int page = 1, string? query = null)
        {
            var actors = _context.Actors.AsQueryable();
            if (query is not null)
            {
                actors = actors.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));
                ViewBag.Query = query;

            }

            // Pagination
            double totalPages = Math.Ceiling(actors.Count() / 3.0);
            actors = actors.Skip((page - 1) * 3).Take(3);
            return View(new ActorsVM()
            {
                Actors = actors.AsEnumerable(),
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
        public IActionResult Create(Actor actor, IFormFile Img)
        {
            if (Img is not null && Img.Length > 0)
            {
                string fileName = CreateFile(Img);

                actor.Img = fileName;
            }
            _context.Actors.Add(actor);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }
        [HttpGet]

        public IActionResult Update(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor is null)
                return NotFound();

            return View(actor);
        }
        [HttpPost]

        public IActionResult Update(Actor actor, IFormFile Img)
        {
            var actorFromDb = _context.Actors.AsNoTracking().FirstOrDefault(e => e.Id == actor.Id);
            if (Img is not null && Img.Length > 0)
            {
                string fileName = CreateFile(Img);
                var oldFilePath = GetOldFilePath(actorFromDb.Img);
                if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                actor.Img = fileName;

            }
            else
            {
                actor.Img = actorFromDb.Img;
            }

            _context.Actors.Update(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)

        {
            var actor = _context.Actors.Find(id);

            if (actor is null)
                return NotFound();

            var oldFilePath = GetOldFilePath(actor.Img);
            if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            _context.Actors.Remove(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        private string CreateFile(IFormFile Img)
        {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + Guid.NewGuid().ToString()
                    + Path.GetExtension(Img.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\actor", fileName);
            using (var stram = System.IO.File.Create(filePath))
            {
                Img.CopyTo(stram);
            }
            return fileName;
        }
        private string GetOldFilePath(string oldFileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\actor", oldFileName);
            return filePath;
        }
    }
}
