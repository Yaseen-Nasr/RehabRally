namespace RehabRally.Web.Core.Dtos
{
    public class TaskDetailsDto
    {
        public int TaskId { get; set; }
        public int ExerciseId { get; set; }
        public int Repetions { get; set; }
        public int Sets { get; set; }
        public int SetsDoneCount { get; set; }
        public bool IsDone { get; set; }
        public string Description { get; set; }
        public List<string>? ImageUrls{ get; set; } = new List<string>();

    }
}
