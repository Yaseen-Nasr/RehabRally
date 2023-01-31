using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RehabRally.Web.Core.AuthModels;
using RehabRally.Web.Core.Models;
using RehabRally.Web.Helpers;
using RehabRally.Web.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RehabRally.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController] 
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthController(UserManager<ApplicationUser> userManager,
                        IOptions<JWT> jwt,
                        RoleManager<IdentityRole> roleManager,
                        IAuthService authService 
                   )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _authService = authService;
    
        }

        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }   
        [HttpGet("domy")]
        [Authorize(AuthenticationSchemes ="Bearer")]
        public async Task<IActionResult> Domy()
        {
            Guid id =Guid.Parse(User.FindFirstValue("uid"));
            return Ok(id);
        }   
       
     }
}
