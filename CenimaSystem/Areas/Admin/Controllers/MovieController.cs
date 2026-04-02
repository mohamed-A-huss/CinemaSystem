using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public MovieController(IWebHostEnvironment env)
        {
            _context = new();
            _env = env;
        }

        public IActionResult Index(int page = 1, string? query = null)
        {
            var movies = _context.Movies
                .Include(m => m.MovieCategories)
                    .ThenInclude(mc => mc.Category)
                .Include(m => m.MovieCinemas)
                    .ThenInclude(mc => mc.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .AsQueryable();
            if (query is not null)
            {
                movies = movies.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));
                ViewBag.Query = query;

            }

            // Pagination
            double totalPages = Math.Ceiling(movies.Count() / 3.0);
            movies = movies.Skip((page - 1) * 3).Take(3);

            

            return View(new MoviesFilterVM()
            {
                Movies = movies.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new MovieVM
            {
                Categories = _context.Categories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                Cinemas = _context.Cinemas
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                Actors = _context.Actors
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public IActionResult Create(MovieVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            string mainImgName = Guid.NewGuid() + Path.GetExtension(vm.MainImg.FileName);
            string path = Path.Combine(_env.WebRootPath, "images/movies", mainImgName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                vm.MainImg.CopyTo(stream);
            }

            var movie = new Movie
            {
                Name = vm.Name,
                Des = vm.Des,
                Price = vm.Price,
                Status = vm.Status,
                DateTime = vm.DateTime,
                MainImg = mainImgName
            };

            // Categories
            foreach (var catId in vm.CategoryIds)
            {
                movie.MovieCategories.Add(new MovieCategory { CategoryId = catId });
            }

            // Cinemas
            foreach (var cinemaId in vm.CinemaIds)
            {
                movie.MovieCinemas.Add(new MovieCinema { CinemaId = cinemaId });
            }

            // Actors
            foreach (var actorId in vm.ActorIds)
            {
                movie.MovieActors.Add(new MovieActor { ActorId = actorId });
            }

            // Sub Images
            if (vm.SubImages != null)
            {
                foreach (var file in vm.SubImages)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(_env.WebRootPath, "images/movies", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    movie.SubImages.Add(new MovieImage { ImgUrl = fileName });
                }
            }

            _context.Movies.Add(movie);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _context.Movies
                .Include(m => m.MovieCategories)
                .Include(m => m.MovieCinemas)
                .Include(m => m.MovieActors)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return NotFound();
            
            var vm = new MovieVM
            {
                Name = movie.Name,
                Des = movie.Des,
                Price = movie.Price,
                Status = movie.Status,
                DateTime = movie.DateTime,

                CategoryIds = movie.MovieCategories.Select(mc => mc.CategoryId).ToList(),
                CinemaIds = movie.MovieCinemas.Select(mc => mc.CinemaId).ToList(),
                ActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList(),

                Categories = _context.Categories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                Cinemas = _context.Cinemas
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                Actors = _context.Actors
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList()
            };
            ViewBag.CurrentImg = movie.MainImg;

            return View(vm);
        }
        [HttpPost]
        public IActionResult Update(int id, MovieVM vm)
        {
            var movie = _context.Movies
                .Include(m => m.MovieCategories)
                .Include(m => m.MovieCinemas)
                .Include(m => m.MovieActors)
                .Include(m => m.SubImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return NotFound();

            // Update basic fields
            movie.Name = vm.Name;
            movie.Des = vm.Des;
            movie.Price = vm.Price;
            movie.Status = vm.Status;
            movie.DateTime = vm.DateTime;

            //  Remove old relations
            movie.MovieCategories.Clear();
            movie.MovieCinemas.Clear();
            movie.MovieActors.Clear();

            //  Add new relations

            foreach (var catId in vm.CategoryIds)
            {
                movie.MovieCategories.Add(new MovieCategory { CategoryId = catId });
            }

            foreach (var cinemaId in vm.CinemaIds)
            {
                movie.MovieCinemas.Add(new MovieCinema { CinemaId = cinemaId });
            }

            foreach (var actorId in vm.ActorIds)
            {
                movie.MovieActors.Add(new MovieActor { ActorId = actorId });
            }

            //  Update Main Image 
            if (vm.MainImg != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(vm.MainImg.FileName);
                string path = Path.Combine(_env.WebRootPath, "images/movies", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    vm.MainImg.CopyTo(stream);
                }

                movie.MainImg = fileName;
            }

            //  Add new sub images
            if (vm.SubImages != null)
            {
                foreach (var file in vm.SubImages)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(_env.WebRootPath, "images/movies", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    movie.SubImages.Add(new MovieImage { ImgUrl = fileName });
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies
                .Include(m => m.MovieCategories)
                .Include(m => m.MovieCinemas)
                .Include(m => m.MovieActors)
                .Include(m => m.SubImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return NotFound();
            foreach (var img in movie.SubImages)
            {
                var path = Path.Combine(_env.WebRootPath, "images/movies", img.ImgUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            // Remove relations
            movie.MovieCategories.Clear();
            movie.MovieCinemas.Clear();
            movie.MovieActors.Clear();

            // Remove sub images
            _context.MovieImages.RemoveRange(movie.SubImages);
            
            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
