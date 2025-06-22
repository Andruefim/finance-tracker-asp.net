

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AngularWithASP.Server.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Errors.Model;

namespace AngularWithASP.Server.Services;

public sealed class RegisterResponseModel
{
    public string Message { get; init; } = default!;
}

public interface IAuthenticateService {
    Task<LoginResponseModel> LoginAsync(LoginModel model);
    Task<RegisterResponseModel> RegisterAsync (RegisterModel model);
    Task<UserModel> GetUserAsync(ClaimsPrincipal userClaims);
}

public class AuthenticateService : IAuthenticateService {
    private readonly UserManager<UserModel> _userManager;
    private readonly IEmailSender _sender;
    private readonly IConfiguration _configuration;

    public AuthenticateService(UserManager<UserModel> userManager, IEmailSender sender, IConfiguration configuration)
    {
        _userManager = userManager;
        _sender = sender;
        _configuration = configuration;
    }

    public async Task<LoginResponseModel> LoginAsync(LoginModel model) { 
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null && !await _userManager.CheckPasswordAsync(user, model.Password)) {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.Id)
        };

        var token = GetToken(authClaims);

        return new LoginResponseModel
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo,
            Email = user.Email,
            Username = user.UserName,
            UserId = user.Id,
        };
    }

    public async Task<RegisterResponseModel> RegisterAsync(RegisterModel model)
    {
        var userExists = await _userManager.FindByEmailAsync(model.Email);

        if (userExists == null)
        {
            throw new BadRequestException("User already exists!");
        }

        var user = new UserModel()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
        };

        var result  = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded) {
            throw new BadRequestException(result.Errors.ToString());
        }

        return new RegisterResponseModel{ Message = "User registered successfully" };
    }

    public async Task<UserModel> GetUserAsync(ClaimsPrincipal userClaims)
    {
        if (userClaims.Identity is null || !userClaims.Identity.IsAuthenticated)
        {
            throw new UnauthorizedException("User is not authenticated");
        }

        var userEmail = userClaims.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null)
        {
            throw new UnauthorizedException("Email claim is missing");
        }

        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return user;
    }

    public JwtSecurityToken GetToken(List<Claim> authClaims)
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
