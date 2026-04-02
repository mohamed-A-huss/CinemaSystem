namespace CinemaSystem.ViewModels
{
    public class MoviesFilterVM
    {
        public IEnumerable<Movie> Movies { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
