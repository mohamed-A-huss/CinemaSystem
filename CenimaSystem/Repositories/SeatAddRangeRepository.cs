namespace CinemaSystem.Repositories
{
    
    public class SeatAddRangeRepository : Repository<Seat>, ISeatAddRangeRepository
    {
        public SeatAddRangeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void AddRange(IEnumerable<Seat> seats)
        {
            foreach (var item in seats)
            {
                _context.Seats.Add(item);
            }
        }
    }

    
}
