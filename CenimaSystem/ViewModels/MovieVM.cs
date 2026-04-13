using CinemaSystem.Validations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CinemaSystem.ViewModels
{
    public class MovieVM
    {
        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;
        public string? Des { get; set; }
        [CustomRangeAtribute(5.0,1000.0)]
        public double Price { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }

        public IFormFile? MainImg { get; set; } 
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
