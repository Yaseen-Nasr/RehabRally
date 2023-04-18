using RehabRally.Core.Consts;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RehabRally.Core.ViewModels
{
    public class ResetPasswordFormViewModel
    {
        public string Id { get; set; }
        [StringLength(100, ErrorMessage = Errors.MaxMinLength, MinimumLength = 6),
         RegularExpression(RegexPatterns.Password, ErrorMessage = Errors.WeekPassword)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Display(Name = "Confirm password"), DataType(DataType.Password),
            Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch)]
        public string ConfirmPassword { get; set; } = null!;
    }

}
