using Microsoft.EntityFrameworkCore;

namespace AngularWithASP.Server.Models;

public class TransactionsContext : DbContext
{
    public TransactionsContext(DbContextOptions<TransactionsContext> options)
        : base(options)
    { 
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;
}
