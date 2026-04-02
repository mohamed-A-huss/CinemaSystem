namespace CinemaSystem.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Img { get; set; }
        // Relationships
        public List<MovieActor> MovieActors { get; set; } = new();

    }
}
