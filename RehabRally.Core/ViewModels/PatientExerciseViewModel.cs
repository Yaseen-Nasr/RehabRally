namespace RehabRally.Core.ViewModels
{
    public class PatientExerciseViewModel
    {
        public string Exercise { get; set; } = null!;
        public int Sets { get; set; }
        public int Repetions { get; set; }
        public bool IsDone { get; set; } 
        public DateTime CreatedOn { get; set; }
        public int SetsDoneCount { get; set; }

    }
}
