using Microsoft.EntityFrameworkCore;
using RehabRally.Web.Core.Consts;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace RehabRally.Web.Core.Models
{
    [Index(nameof(Title), nameof(CategoryId), IsUnique = true)] 
    public class Exercise : BaseModel
    {
        public int Id { get; set; }
        [MaxLength(300)]
        public string Title { get; set; } = null!;
        public string? ImageUrl { get; set; } 
        public string? ImageThumbnailUrl { get; set; } 
        public string? ImageLinkUrl { get; set; } 
        public string? ImageSecondaryUrl { get; set; } 
        public string Description { get; set; } = null!;  
        public int CategoryId { get; set; }
        public Category? Category { get; set; } 
        public string? ImageThirdUrl { get; set; } 
        public string? ImageFourthUrl { get; set; }
    }
}
 
