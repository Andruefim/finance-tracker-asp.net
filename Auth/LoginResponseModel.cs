using NuGet.Common;

namespace AngularWithASP.Server.Auth;

public class LoginResponseModel
{
    public string Token { get; set; }
    public DateTime Expiration {  get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string UserId { get; set; }
}
