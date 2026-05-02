using CinemaSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CinemaSystem.ViewModels;

namespace CinemaSystem.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }

        public DbSet<Show> Shows { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }


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


            modelBuilder.Entity<BookingSeat>()
        .HasOne(bs => bs.Seat)
        .WithMany()
        .HasForeignKey(bs => bs.SeatId)
        .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Booking>()
    .HasOne(b => b.User)
    .WithMany()
    .HasForeignKey(b => b.UserId);

        }
        public DbSet<CinemaSystem.ViewModels.RegisterVM> RegisterVM { get; set; } = default!;
        public DbSet<CinemaSystem.ViewModels.LoginVM> LoginVM { get; set; } = default!;
        public DbSet<CinemaSystem.ViewModels.ResendEmailConfirmationVM> ResendEmialConfirmatiomVM { get; set; } = default!;
        public DbSet<CinemaSystem.ViewModels.ForgetPasswordVM> ForgetPasswordVM { get; set; } = default!;
        public DbSet<CinemaSystem.ViewModels.ValidateOTPVM> ValidateOTPVM { get; set; } = default!;
        public DbSet<CinemaSystem.ViewModels.ChangePasswordVM> ChangePasswordVM { get; set; } = default!;
    }
}
