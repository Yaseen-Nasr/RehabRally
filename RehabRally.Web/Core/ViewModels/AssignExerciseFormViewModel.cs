using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RehabRally.Web.Core.ViewModels
{
    public class AssignExerciseFormViewModel
    { 
        public int? Id { get; set; }
        [Display(Name = "Exercise")] 
        public int ExerciseId{ get; set; }
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        public string UserId { get; set; } = null!;
        public IEnumerable<SelectListItem>? Exercises{ get; set; }
        public IEnumerable<SelectListItem>? Categories{ get; set; } 
        public int Sets { get; set; }
        public int Repetions { get; set; }

    }
}
