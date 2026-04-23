namespace CinemaSystem.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public int Count { get; set; }
        public double PricePerProduct { get; set; }
        public double TotalPrice { get; set; }
    }
}
