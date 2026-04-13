using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required()]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; } = null!;
        public string? Img { get; set; }
        // Relationships
        public List<MovieCinema> MovieCinemas { get; set; } = new();

    }
}
