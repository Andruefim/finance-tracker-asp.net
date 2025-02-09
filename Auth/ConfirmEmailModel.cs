using System.ComponentModel.DataAnnotations;

namespace AngularWithASP.Server.Auth;

public class ConfirmEmailModel
{
    [Required(ErrorMessage = "Confirmation code is required")]
    public string? Code { get; set; }
}
