namespace CinemaSystem.Models
{
    public class FavoriteMovie
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public double PricePerProduct { get; set; }
    }
}
