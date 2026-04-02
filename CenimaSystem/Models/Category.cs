namespace CinemaSystem.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        // Relationships
        public List<MovieCategory> MovieCategories { get; set; } = new();

    }
}
