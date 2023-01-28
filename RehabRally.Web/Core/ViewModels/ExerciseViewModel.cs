namespace RehabRally.Web.Core.ViewModels
{
    public class ExerciseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!; 
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }
        public string Description { get; set; } = null!; 
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
