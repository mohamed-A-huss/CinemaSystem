using CinemaSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.DataAccess
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }

        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieCategory> MovieCategories { get; set; }
        public DbSet<MovieCinema> MovieCinemas { get; set; }

        
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer
        //        ("Data Source=DESKTOP-90JQGSU\\SQLEXPRESS;Initial Catalog=CinemaDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            modelBuilder.Entity<MovieCategory>()
                .HasKey(mc => new { mc.MovieId, mc.CategoryId });

            modelBuilder.Entity<MovieCinema>()
                .HasKey(mc => new { mc.MovieId, mc.CinemaId });
        }
    }
}
