using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using RehabRally.Web.Core.Consts;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace RehabRally.Web.Core.ViewModels
{
    public class ExerciseFormViewModel
    {
        public int Id { get; set; } 
        [MaxLength(500, ErrorMessage = Errors.MaxLength)]
        [Remote("AllowItem", null, AdditionalFields = "Id,AuthorId"
            , ErrorMessage = Errors.DuplicatedBook)]
        public string Title { get; set; } = null!; 
        public IFormFile? Image { get; set; } 
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }
        [Display(Name = "Default Image Link")]  
        public string? ImageLinkUrl { get; set; }
        [Display(Name = "Secoundary Image Link")]
        [AssertThat("ImageLinkUrl != null", ErrorMessage = Errors.EmptyDefaultLinkImege)]
        public string? ImageSecondaryUrl { get; set; }
        [Display(Name = "Third Image Link")]
        [AssertThat("ImageSecondaryUrl  != null", ErrorMessage = Errors.EmptyPreviousLinkImege)]
        public string? ImageThirdUrl { get; set; }
        [Display(Name = "Fourth Image Link")]
        [AssertThat("ImageThirdUrl != null", ErrorMessage = Errors.EmptyPreviousLinkImege)]
        public string? ImageFourthUrl { get; set; }
        public string Description { get; set; } = null!; 
        [Display(Name = "Category")]
        [Remote("AllowItem", null, AdditionalFields = "Id,Title",
          ErrorMessage = Errors.DuplicatedCategory)]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories{ get; set; }
 
    }
}
