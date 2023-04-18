using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RehabRally.Web.Core.AuthModels;
using RehabRally.Core.Models;
  
using RehabRally.Ef.Data;
using RehabRally.Core.Abstractions;

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
        private readonly IUnitOfWork _unitOfWork;

        private readonly FirebaseMessaging _firebaseMessaging;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager,
                        IOptions<JWT> jwt,
                        RoleManager<IdentityRole> roleManager,
                        IAuthService authService,
                        FirebaseMessaging firebaseMessaging,
                        ILogger<AuthController> logger,
                        IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _authService = authService;
            _firebaseMessaging = firebaseMessaging;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string firebaseToken = HttpContext.Request.Headers["FirebaseToken"];

            if (string.IsNullOrEmpty(firebaseToken))
                return BadRequest("Invalid firebaseToken");

            var result = await _authService.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            var mashine = await _unitOfWork.RegisterdMashines.GetQueryable(x => x.FirebaseToken == firebaseToken).FirstOrDefaultAsync();
            if (mashine is not null)
                mashine.UserId = result.UserId;
            else
                await _unitOfWork.RegisterdMashines.Add(new RegisterdMashine { UserId = result.UserId, FirebaseToken = firebaseToken });

              _unitOfWork.Complete();

            return Ok(result);
        }


        [HttpGet("userInfo")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> UserInfo()
        {
            string id = User.FindFirstValue("uid");
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return BadRequest("Some Thing went wrong");

            return Ok(new
            {
                user.FullName,
                user.Email,
                user.UserName,
                user.MobileNumber,
                user.Age,
            });
        }
         

    }
}
