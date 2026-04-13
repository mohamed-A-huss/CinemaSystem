namespace CinemaSystem.Repositories.IRepositories
{
    
    public interface IMovieSubImgRepository : IRepository<MovieImage>
    {
        void DeleteRange(IEnumerable<MovieImage> productSubImgs);
    }
}
