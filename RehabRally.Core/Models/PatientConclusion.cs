namespace RehabRally.Core.Models
{
    public class PatientConclusion
    { 
        public int Id { get; set;}
        public string Precaution { get; set; } = null!; 
        public string UserId { get; set; } = null!; 
    }
}
