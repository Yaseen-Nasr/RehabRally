﻿namespace RehabRally.Web.Core.Models
{
    public class PatientConclusion
    {
         public byte Id { get; set;}
        public string Conclusion { get; set; } = null!; 
        public string UserId { get; set; } = null!; 
    }
}
