namespace CinemaSystem.Repositories
{
    
    public class MovieSubImgRepository : Repository<MovieImage>, IMovieSubImgRepository
    {
        public MovieSubImgRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void DeleteRange(IEnumerable<MovieImage> movieSubImgs)
        {
            foreach (var item in movieSubImgs)
            {
                _context.MovieImages.Remove(item);
            }
        }
    }
}
