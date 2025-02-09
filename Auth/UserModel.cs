using Microsoft.AspNetCore.Identity;

namespace AngularWithASP.Server.Auth;

public class UserModel : IdentityUser
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Token { get; set; }
    public bool? EmailConfirmed { get; set; } = false;
    public string? EmailConfirmationCode { get; set; }

    public long? TotalBalance { get; set; }

}
