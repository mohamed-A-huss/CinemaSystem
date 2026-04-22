namespace CinemaSystem.ViewModels
{
    public class ApplicationUserVM
    {
        [Required]
        public string FirstName { get; set; } 
        [Required]
        public string LastName { get; set; } 
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Address { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
