namespace AngularWithASP.Server.Models;

public class Transaction
{
    public long Id { get; set; }
    public string? Date { get; set; }
    public string? Category { get; set; }
    public long Amount {  get; set; }
    public string? Description { get; set; }
}
 