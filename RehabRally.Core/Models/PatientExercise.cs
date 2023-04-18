using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RehabRally.Core.Models
{

    public class PatientExercise
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int Sets { get; set; }
        public int Repetions { get; set; }
        public int SetsDoneCount { get; set; } 
        public int ExerciseId { get; set; } 
        public  Exercise? Exercise { get; set; }
        public  bool IsDone{ get; set; }
        public DateTime CreatedOn { get; set; }


    }
}
