namespace CinemaSystem.Models
{
    public class Seat
    {
        public int Id { get; set; }

        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; }

        public string Row { get; set; } = null!; // A, B, C
        public int Number { get; set; }          // 1, 2, 3
    }
}
