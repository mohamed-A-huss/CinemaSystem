using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class ActorController : Controller
    {
        private readonly IRepository<Actor> _repository;// = new();

        public ActorController(IRepository<Actor> repository)
        {
            _repository = repository;
        }

        
        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            var actors =await _repository.GetAsync();
            if (query is not null)
            {
                actors = actors.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));
                

            }

            // Pagination
            double totalPages = Math.Ceiling(actors.Count() / 3.0);
            actors = actors.Skip((page - 1) * 3).Take(3);
            return View(new ActorsVM()
            {
                Actors = actors.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query= query
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Actor());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile Img, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            if (Img is not null && Img.Length > 0)
            {
                string fileName = CreateFile(Img);

                actor.Img = fileName;
            }
            await _repository.CreateAsync(actor, cancellationToken);
            await _repository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Actor Added successfully!";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]

        public async Task<IActionResult> Update(int id, CancellationToken cancellationToken = default)
        {
            var actor = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);
            if (actor is null)
                return NotFound();

            return View(actor);
        }
        [HttpPost]

        public async Task<IActionResult> Update(Actor actor, IFormFile Img, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            var actorFromDb = await _repository.GetOneAsync(e => e.Id == actor.Id, cancellationToken: cancellationToken ,tracked: false);
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


             _repository.Update(actor);
            await _repository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Actor Updated successfully!";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)

        {
            var actor = await _repository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (actor is null)
                return NotFound();

            var oldFilePath = GetOldFilePath(actor.Img);
            if (oldFilePath is not null && System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            _repository.Delete(actor);
            await _repository.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Actor Deleted successfully!";

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
