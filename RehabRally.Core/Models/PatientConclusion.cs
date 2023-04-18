namespace RehabRally.Core.Models
{
    public class PatientConclusion
    { 
        public int Id { get; set;}
        public string Conclusion { get; set; } = null!; 
        public string UserId { get; set; } = null!; 
    }
}
