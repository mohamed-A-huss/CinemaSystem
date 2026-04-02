namespace CinemaSystem.Models
{
    public class MovieImage
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; } = null!;
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
