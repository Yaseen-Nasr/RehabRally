using Microsoft.AspNetCore.Mvc;
using RehabRally.Web.Core.Consts;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace RehabRally.Web.Core.ViewModels
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }
        [StringLength(100, ErrorMessage = Errors.MaxMinLength, MinimumLength = 5), Display(Name = "Full Name"),
            RegularExpression(RegexPatterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string FullName { get; set; } = null!;

        [StringLength(20, ErrorMessage = Errors.MaxMinLength, MinimumLength = 3)]
        [Remote("AllowUserName", controller: null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
        [RegularExpression(RegexPatterns.UserName, ErrorMessage = Errors.InvalidUserName)]
        public string UserName { get; set; } = null!;


        [MaxLength(200, ErrorMessage = Errors.MaxLength), EmailAddress]
        [Remote("AllowEmail", controller: null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
        public string Email { get; set; } = null!; 

        [Phone]
        [Display(Name = "Phone number"), MaxLength(11, ErrorMessage = Errors.MaxLength),
                    RegularExpression(RegexPatterns.MbobileNumber, ErrorMessage = Errors.InvalidPhoneNumber)]
         public string MobileNumber { get; set; } = null!; 
        public int Age { get; set; }  
        [StringLength(100, ErrorMessage = Errors.MaxMinLength, MinimumLength = 6),
            RegularExpression(RegexPatterns.Password, ErrorMessage = Errors.WeekPassword)]
        [DataType(DataType.Password)]
        [RequiredIf("Id==null", ErrorMessage = Errors.RequiredField)]
        public string? Password { get; set; } = null!;

        [Display(Name = "Confirm password"), DataType(DataType.Password),
            Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch)]
        [RequiredIf("Id==null", ErrorMessage = Errors.RequiredField)]
        public string? ConfirmPassword { get; set; } = null!;

    }
}
