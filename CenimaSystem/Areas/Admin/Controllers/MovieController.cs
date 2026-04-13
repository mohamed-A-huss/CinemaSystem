using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    public class MovieController : Controller
    {
        private readonly IRepository<Movie> _movieRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Cinema> _cinemaRepo;
        private readonly IRepository<Actor> _actorRepo;
        private readonly IMovieSubImgRepository _movieImageRepo;
        private readonly IWebHostEnvironment _env;

        public MovieController(
            IRepository<Movie> movieRepo,
            IRepository<Category> categoryRepo,
            IRepository<Cinema> cinemaRepo,
            IRepository<Actor> actorRepo,
            IMovieSubImgRepository movieImageRepo,
            IWebHostEnvironment env)
        {
            _movieRepo = movieRepo;
            _categoryRepo = categoryRepo;
            _cinemaRepo = cinemaRepo;
            _actorRepo = actorRepo;
            _movieImageRepo = movieImageRepo;
            _env = env;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null)
        {
            var movies = await _movieRepo.GetAsync(
                                includes: new Expression<Func<Movie, object>>[]
                                {
                                    m => m.MovieCategories,
                                    m => m.MovieCinemas,
                                    m => m.MovieActors
                                }
                            );
            var queryable = movies.AsQueryable();

            if (query is not null)
            {
                queryable = queryable.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));

            }

            // Pagination
            double totalPages = Math.Ceiling(queryable.Count() / 3.0);
            movies = queryable.Skip((page - 1) * 3).Take(3);

            

            return View(new MoviesFilterVM()
            {
                Movies = movies.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query= query
            });
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            
            var vm = new MovieVM
            {
                Categories = (await _categoryRepo.GetAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                Cinemas = (await _cinemaRepo.GetAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                Actors = (await _actorRepo.GetAsync())
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieVM vm)
        {
            if (!ModelState.IsValid)
                return View(new MovieVM
                {
                    Categories = (await _categoryRepo.GetAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                    Cinemas = (await _cinemaRepo.GetAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                    Actors = (await _actorRepo.GetAsync())
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList()
                });

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

            await _movieRepo.CreateAsync(movie);
            await _movieRepo.CommitAsync();
            TempData["Success"] = "Movie Added successfully!";

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var movie = await _movieRepo.GetOneAsync(
                                m => m.Id == id,
                                includes: new Expression<Func<Movie, object>>[]
                                {
                                    m => m.MovieCategories,
                                    m => m.MovieCinemas,
                                    m => m.MovieActors,
                                    m => m.SubImages
                                }
                            );

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

                Categories = (await _categoryRepo.GetAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                Cinemas = (await _cinemaRepo.GetAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                Actors = (await _actorRepo.GetAsync())
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList()
            };
            ViewBag.CurrentImg = movie.MainImg;

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, MovieVM vm)

        {
            if (!ModelState.IsValid)
                return View(new MovieVM
                {
                    Categories = (await _categoryRepo.GetAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                    Cinemas = (await _cinemaRepo.GetAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),

                    Actors = (await _actorRepo.GetAsync())
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList()
                });
            var movie = await _movieRepo.GetOneAsync(
                                m => m.Id == id,
                                includes: new Expression<Func<Movie, object>>[]
                                {
                                    m => m.MovieCategories,
                                    m => m.MovieCinemas,
                                    m => m.MovieActors,
                                    m => m.SubImages
                                }
                            );

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

            await _movieRepo.CommitAsync();
            TempData["Success"] = "Movie Updated successfully!";

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepo.GetOneAsync(
                                m => m.Id == id,
                                includes: new Expression<Func<Movie, object>>[]
                                {
                                    m => m.MovieCategories,
                                    m => m.MovieCinemas,
                                    m => m.MovieActors,
                                    m => m.SubImages
                                }
                            );

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
             _movieImageRepo.DeleteRange(movie.SubImages);

            _movieRepo.Delete(movie);
            await _movieRepo.CommitAsync();
            TempData["Success"] = "Movie Deleted successfully!";

            return RedirectToAction("Index");
        }
    }
}
