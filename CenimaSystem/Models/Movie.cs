
namespace CinemaSystem.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required()]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }=null!;
        [MinLength(5)]
        [MaxLength(500)]
        public string? Des { get; set; }

        //[CustomRangeAtribute(5.0,1000.0)]
        public double Price { get; set; }
        public bool Status { get; set; } = true;
        public DateTime DateTime { get; set; }

        public string MainImg { get; set; }=null!;
        public string? Trailer { get; set; }

        // Relationships
        public List<MovieCategory> MovieCategories { get; set; } = new();
        public List<MovieCinema> MovieCinemas { get; set; } = new();
        public List<MovieActor> MovieActors { get; set; } = new();
        public List<MovieImage> SubImages { get; set; } = new();


    }
}
