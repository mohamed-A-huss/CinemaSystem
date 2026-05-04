using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace CinemaSystem.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _movieRepo;
        private readonly IRepository<Seat> _seatRepo;
        private readonly ISeatAddRangeRepository _seatAddRangeRepo;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;




        public HomeController(IRepository<Movie> movieRepo, IRepository<Seat> seatRepo, ISeatAddRangeRepository seatAddRangeRepo, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _movieRepo = movieRepo;
            _seatRepo = seatRepo;
            _seatAddRangeRepo = seatAddRangeRepo;
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        {
            var movie = await _movieRepo.GetOneAsync(
                            m => m.Id == id,
                            include: query => query
                                .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
                                .Include(m => m.MovieCinemas).ThenInclude(mc => mc.Cinema)
                                .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
                                .Include(m => m.SubImages),
                            cancellationToken: cancellationToken
                        );

            if (movie == null)
            {
                return NotFound();
            }
             var shows = await _context.Shows
            .Where(s => s.MovieId == id)
            .Include(s => s.Cinema)
            .ToListAsync();

            ViewBag.Shows = shows;
            //            var shows = new List<Show>
            //{
            //    new Show { MovieId = 3, CinemaId = 5, StartTime = DateTime.Now.AddHours(2) },
            //    new Show { MovieId = 4, CinemaId = 5, StartTime = DateTime.Now.AddHours(5) },
            //    new Show { MovieId = 5, CinemaId = 5, StartTime = DateTime.Now.AddHours(3) }
            //};

            //            _context.Shows.AddRange(shows);
            //            await _context.SaveChangesAsync();

            return View(movie);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Book(int showId)
        {
            var show = await _context.Shows
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .FirstOrDefaultAsync(s => s.Id == showId);
            var bookedSeatIds = await _context.BookingSeats
                                    .Where(bs => bs.Booking.ShowId == showId)
                                    .Select(bs => bs.SeatId)
                                    .ToListAsync();
            var seats = await GetAvailableSeats(showId);

            ViewBag.BookedSeats = bookedSeatIds;
            ViewBag.Seats = seats;
            return View(show);
        }
        public async Task<List<Seat>> GetAvailableSeats(int showId)
        {
            var show = await _context.Shows
                .Include(s => s.Cinema)
                .FirstOrDefaultAsync(s => s.Id == showId);

            var bookedSeatIds = await _context.BookingSeats
                .Where(bs => bs.Booking.ShowId == showId)
                .Select(bs => bs.SeatId)
                .ToListAsync();

            var seats = await _context.Seats
                .Where(s => s.CinemaId == show.CinemaId)
                .ToListAsync();

            return seats.Where(s => !bookedSeatIds.Contains(s.Id)).ToList();
        }
        [HttpPost]
        [Authorize]



        public async Task<IActionResult> ConfirmBooking(int showId, List<int> seatIds)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var show = await _context.Shows
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == showId);

            var price = show.Price ?? show.Movie.Price;

            
            var booking = new Booking
            {
                ShowId = showId,
                UserId = user.Id,
                TotalPrice = seatIds.Count * price,
                BookingSeats = seatIds.Select(id => new BookingSeat
                {
                    SeatId = id
                }).ToList()
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },

                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                Quantity = seatIds.Count,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "egp",
                    UnitAmount = (long)(price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = show.Movie.Name,
                        Description = $"Booking {seatIds.Count} seats"
                    }
                }
            }
        },

                Mode = "payment",

                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Booking/Success?bookingId={booking.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Booking/Cancel?bookingId={booking.Id}",
            };

            var service = new SessionService();
            var session = service.Create(options);

            booking.SessionId = session.Id;
            booking.PaymentIntentId = session.PaymentIntentId;
            await _context.SaveChangesAsync();

            return Redirect(session.Url);
        }
        public async Task<IActionResult> MyTickets()
        {
            var user = await _userManager.GetUserAsync(User);

            var bookings = await _context.Bookings
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Show)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Show)
                    .ThenInclude(s => s.Cinema)
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return View(bookings);
        }
        [HttpPost]
        public async Task<IActionResult> Refund(int bookingId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.Status != "Paid")
                return BadRequest();

            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = booking.PaymentIntentId,
            };

            var service = new RefundService();
            var refund = service.Create(refundOptions);

            booking.Status = "Refunded";
            booking.IsPaid = false;

            await _context.SaveChangesAsync();

            return RedirectToAction("MyTickets");
        }
    }
}
