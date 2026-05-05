using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace CinemaSystem.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Success(int bookingId, string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
                return BadRequest("Missing session_id");

            var service = new SessionService();
            var session = service.Get(session_id, new SessionGetOptions
            {
                Expand = new List<string> { "payment_intent" }
            });

            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return NotFound();

            if (string.IsNullOrEmpty(session.PaymentIntentId))
                return BadRequest("Payment not completed");

            booking.PaymentIntentId = session.PaymentIntentId;
            booking.Status = "Paid";
            booking.IsPaid = true;

            await _context.SaveChangesAsync();

            return View();
        }
        public async Task<IActionResult> Cancel(int bookingId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return View();
        }
    }
}
