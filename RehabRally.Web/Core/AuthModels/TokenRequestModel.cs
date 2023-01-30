using System.ComponentModel.DataAnnotations;

namespace RehabRally.Web.Core.AuthModels
{
    public class TokenRequestModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
