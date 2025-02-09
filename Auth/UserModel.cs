using Microsoft.AspNetCore.Identity;

namespace AngularWithASP.Server.Auth;

public class UserModel : IdentityUser
{
    public string? Token { get; set; }
    public string? EmailConfirmationCode { get; set; }

    public long? TotalBalance { get; set; }

}
