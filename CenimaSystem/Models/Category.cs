using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required()]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; } = null!;
        // Relationships
        public List<MovieCategory> MovieCategories { get; set; } = new();

    }
}
