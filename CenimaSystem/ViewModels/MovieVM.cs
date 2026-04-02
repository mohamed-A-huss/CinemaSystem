using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaSystem.ViewModels
{
    public class MovieVM
    {
        public string Name { get; set; } = null!;
        public string? Des { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }

        public IFormFile MainImg { get; set; } = null!;
        public List<IFormFile>? SubImages { get; set; }

        public List<int> CategoryIds { get; set; } = new();
        public List<int> CinemaIds { get; set; } = new();
        public List<int> ActorIds { get; set; } = new();

        // For dropdowns
        public List<SelectListItem>? Categories { get; set; }
        public List<SelectListItem>? Cinemas { get; set; }
        public List<SelectListItem>? Actors { get; set; }
    }
}
