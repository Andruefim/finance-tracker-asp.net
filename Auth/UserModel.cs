namespace AngularWithASP.Server.Auth;

public class UserModel
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Token { get; set; }

    public long? TotalBalance { get; set; }

}
