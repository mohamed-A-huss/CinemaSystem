namespace CinemaSystem.Models
{
    public class Show
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        public DateTime StartTime { get; set; }
        public double? Price { get; set; }

    }
}
