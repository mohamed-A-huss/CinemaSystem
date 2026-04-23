namespace CinemaSystem.Models
{
    public class UserReview
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public string Comment { get; set; } = string.Empty;
        public int Rate { get; set; }
    }
}
