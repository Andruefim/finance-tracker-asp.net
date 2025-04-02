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
        private readonly IConfiguration _configuration;

        public AuthenticateController(
            UserManager<UserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _sender = emailSender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password)) {
                return Unauthorized("Invalid email or password");
            }

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id)
            };

            var token = GetToken(authClaims);

            return Ok(new 
            { 
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                email = user.Email,
                username = user.UserName,
                userId = user.Id,
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null) {
                return BadRequest("User already exists!");
            }

            var user = new UserModel()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "User registered successfully" });
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            if (User.Identity is null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated");
            }

            // Get the user's email from the claims
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null) 
            {
                return Unauthorized("Email claim is missing");
            }

            // Fetch the user from the database
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) 
            { 
                return NotFound("User not found");
            }

            // Return the user details
            return Ok(new
            {
                email = user.Email,
                username = user.UserName,
                userId = user.Id,
                emailConfirmed = user.EmailConfirmed,
            });
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
            var subject = "Finance tracker";

            var message = $"We received your registration request. Verification code is {code}. Proceed with your application on the website.";
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

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
