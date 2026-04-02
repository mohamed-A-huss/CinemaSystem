namespace CinemaSystem.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }=null!; 
        public string? Des { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; } = true;
        public DateTime DateTime { get; set; }

        public string MainImg { get; set; } = null!;
        public string? Trailer { get; set; }

        // Relationships
        public List<MovieCategory> MovieCategories { get; set; } = new();
        public List<MovieCinema> MovieCinemas { get; set; } = new();
        public List<MovieActor> MovieActors { get; set; } = new();
        public List<MovieImage> SubImages { get; set; } = new();


    }
}
