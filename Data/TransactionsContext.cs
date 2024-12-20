using AngularWithASP.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularWithASP.Server.Data;

public class TransactionsContext : DbContext
{
    public TransactionsContext(DbContextOptions<TransactionsContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;
}
