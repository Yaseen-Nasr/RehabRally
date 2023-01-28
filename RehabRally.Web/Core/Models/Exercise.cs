using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RehabRally.Web.Core.Models
{
    [Index(nameof(Title), nameof(CategoryId), IsUnique = true)] 
    public class Exercise : BaseModel
    {
        public int Id { get; set; }
        [MaxLength(300)]
        public string Title { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; }
        public string? ImageThumbnailUrl { get; set; } 
        public string Description { get; set; } = null!;  
        public int CategoryId { get; set; }
        public Category? Category { get; set; } 
    }
}
 
