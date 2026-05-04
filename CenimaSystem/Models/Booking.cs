namespace CinemaSystem.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int ShowId { get; set; }
        public Show Show { get; set; }

        public DateTime BookingTime { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public double TotalPrice { get; set; }
        public string? SessionId { get; set; }
        public bool IsPaid { get; set; } = false;
        public string Status { get; set; } = "Pending";
        public string? PaymentIntentId { get; set; }

        public List<BookingSeat> BookingSeats { get; set; } = new();
    }
}
