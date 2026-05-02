using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Admin.Controllers
{
    [Area(SD.AdminArea)]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Seat> _seatRepo;
        private readonly ISeatAddRangeRepository _seatAddRangeRepo;

        public HomeController(ApplicationDbContext context, IRepository<Seat> seatRepo, ISeatAddRangeRepository seatAddRangeRepo)
        {
            _context = context;
            _seatRepo = seatRepo;
            _seatAddRangeRepo = seatAddRangeRepo;
        }
        public IActionResult Index()
        {
            ViewBag.Movies = _context.Movies.Count();
            ViewBag.Categories = _context.Categories.Count();
            ViewBag.Actors = _context.Actors.Count();
            ViewBag.Cinemas = _context.Cinemas.Count();

            return View();
        }
        public async Task GenerateSeats(int cinemaId)
        {
            var rows = new[] { "A", "B", "C", "D", "E", "F", "G", "H" };
            int seatsPerRow = 12;

            var seats = new List<Seat>();

            foreach (var row in rows)
            {
                for (int i = 1; i <= seatsPerRow; i++)
                {
                    seats.Add(new Seat
                    {
                        CinemaId = cinemaId,
                        Row = row,
                        Number = i
                    });
                }
            }

            _seatAddRangeRepo.AddRange(seats);

            await _seatRepo.CommitAsync();
        }

    }
}
