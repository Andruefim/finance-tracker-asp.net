using AngularWithASP.Server.Auth;
using AngularWithASP.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AngularWithASP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _sender;
        private readonly IAuthenticateService _authenticateService;

        public AuthenticateController(
                UserManager<UserModel> userManager,
                RoleManager<IdentityRole> roleManager,
                IAuthenticateService authenticateService,
                IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _sender = emailSender;
            _authenticateService = authenticateService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var loginResponse = await _authenticateService.LoginAsync(model);

            return Ok(loginResponse);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var registerResponse = await _authenticateService.RegisterAsync(model);

            return Ok(registerResponse);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            var user = await _authenticateService.GetUserAsync(User);

            // Return the user details
            return Ok(user);
        }

        [HttpPost("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return Unauthorized("Email claim is missing");
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var code = new Random().Next(10000, 999999).ToString();

            user.EmailConfirmationCode = code;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest(new { message = "Failed to save confirmation code", errors = updateResult.Errors });
            }

            var toEmail = userEmail;
            var subject = "Sending with SendGrid";

            var message = $"Your confirmation code is {code}.";
            await _sender.SendEmailAsync(toEmail, subject, message);

            return Ok(new { confirmationSent = true, userCode = user.EmailConfirmationCode });
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailModel confirmation)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            { 
                return Unauthorized("Email claim is missing");
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            { 
                return NotFound("User not found");
            }

            Console.WriteLine("debugCode", confirmation.Code);
            Console.WriteLine("debugEmailCode", user.EmailConfirmationCode);

            if (confirmation.Code != user.EmailConfirmationCode) {
                return BadRequest($"Invalid confirmation code.{confirmation.Code} is not {user.EmailConfirmationCode}");
            }

            user.EmailConfirmed = true;
            //user.EmailConfirmationCode = null;
            await _userManager.UpdateAsync(user);

            return Ok(new { emailConfirmed = true });
            
        }
    }
}
