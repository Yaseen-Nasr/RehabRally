using Microsoft.AspNetCore.Mvc;
using RehabRally.Core.Consts;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RehabRally.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Category")]
        [Remote("AllowItem", null, AdditionalFields = "Id"
            , ErrorMessage = Errors.Duplicated),
            RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string Name { get; set; } = null!;
    }
}
