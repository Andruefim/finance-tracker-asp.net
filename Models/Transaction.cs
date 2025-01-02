using AngularWithASP.Server.Interfaces;
using static AngularWithASP.Server.Attributes.ValidationAttributes;

namespace AngularWithASP.Server.Models;

public class Transaction : IUserOwned
{
    public long Id { get; set; }
    public string? Date { get; set; }
    public string? Category { get; set; }

    [ValidTransactionAmount("Transaction amount must be greater than 0")]
    public long Amount {  get; set; }
    public string? Description { get; set; }
    public string? UserId { get; set; }
}
 