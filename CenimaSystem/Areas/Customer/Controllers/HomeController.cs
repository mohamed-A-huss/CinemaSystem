using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _movieRepo;
       

        public HomeController(IRepository<Movie> movieRepo
            )
        {
            _movieRepo = movieRepo;
            
        }
        
        public async Task<IActionResult> Index(int page = 1, string? query = null, CancellationToken cancellationToken = default)
        {
            var movies = await _movieRepo.GetAsync(cancellationToken: cancellationToken);
            var queryable = movies.AsQueryable();

            if (query is not null)
            {
                queryable = queryable.Where(e => e.Name.ToLower().Contains(query.Trim().ToLower()));

            }

            // Pagination
           
            double totalPages = Math.Ceiling(queryable.Count() / 12.0);
            movies = queryable.Skip((page - 1) * 12).Take(12);



            return View(new MoviesFilterVM()
            {
                Movies = movies.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query = query
            });
        }
    }
}
