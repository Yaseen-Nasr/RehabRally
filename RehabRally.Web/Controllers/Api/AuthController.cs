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
using RehabRally.Web.Core.Models;
using RehabRally.Web.Data;
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
        private readonly ApplicationDbContext _context;
        private readonly FirebaseMessaging _firebaseMessaging;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> userManager,
                        IOptions<JWT> jwt,
                        RoleManager<IdentityRole> roleManager,
                        IAuthService authService
,
                        ApplicationDbContext context,
                        FirebaseMessaging firebaseMessaging,
                        ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _authService = authService;
            _context = context;
            _firebaseMessaging = firebaseMessaging;
            _logger = logger;
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

            var mashine = await _context.RegisterdMashines.Where(x => x.FirebaseToken == firebaseToken).FirstOrDefaultAsync();
            if (mashine is not null)
                mashine.UserId = result.UserId;
            else
                await _context.RegisterdMashines.AddAsync(new RegisterdMashine { UserId = result.UserId, FirebaseToken = firebaseToken });

            await _context.SaveChangesAsync();

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


        [HttpGet("TestFcm")]
        [AllowAnonymous]
        public async Task<IActionResult> TestFcm()
        {
            var message = new Message()
            {
                Notification = new Notification()
                {
                    Title = "RehabRally",
                    Body = "Yarab",

                },

                Token = "ehM0pAY_RDS0IKL1-XILCf:APA91bG5FRknTOQe4WrdHmqbZiKf2Evx0OoKx2n4a56RRvuDbcSNhVSViKPWAQCrvQFqX6ZYPUI25SizWt0ktZ8ViYDvBytacUXoFxy69lfMVylNR7xBFm63aapjXt0FlDkIRQHVrJma",

                Data = new Dictionary<string, string>
                         {
                             { "Title", "RehabRally" },
                             { "Body", "Yarab" },
                             { "Type", "Task" },
                          }
            };

            var result = await _firebaseMessaging.SendAsync(message);
            if (result != null)
            {

                _logger.LogInformation($"Push notification sent to token ");
                return Ok(result);
            }
            else
            {
                _logger.LogError($"Failed to send push notification to token ");
                return StatusCode(500);
            }

        }

    }
}
