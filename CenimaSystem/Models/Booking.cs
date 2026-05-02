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

        public List<BookingSeat> BookingSeats { get; set; } = new();
    }
}
