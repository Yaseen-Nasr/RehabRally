using RehabRally.Core.Models;

namespace RehabRally.Core.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string MobileNumber { get; set; } = null!;
        public int Age { get; set; }
        public List<string> Precautions { get; set; } =new List<string>();
        public string Conclusion { get; set; } 
        public AssignExerciseFormViewModel? AssignExercise { get; set; }
        public IEnumerable<PatientExerciseViewModel> PatientExercises { get; set; } = null!; 
    }
}
