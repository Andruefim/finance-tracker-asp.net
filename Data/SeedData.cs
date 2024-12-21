using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using AngularWithASP.Server.Models;

namespace AngularWithASP.Server.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new TransactionsContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<TransactionsContext>>()))
        {
            // Look for any transactions.
            if (context.Transactions.Any())
            {
                return; // DB has been seeded
            }
            context.Transactions.AddRange(
                new Transaction
                {
                    Amount = -20534,
                    Category = "Transport",
                    Date = "2024-11-19T19:58:19.162Z",
                    Description = "transport desc",
                    Id = 1
                },
                new Transaction
                {
                    Amount = -30534,
                    Category = "Shopping",
                    Date = "2024-11-19T19:58:27.944Z",
                    Description = "Shopping desc",
                    Id = 2
                },
                new Transaction
                {
                    Amount = 40534,
                    Category = "Transport",
                    Date = "2024-11-19T19:58:19.162Z",
                    Description = "transport desc",
                    Id = 3
                },
                new Transaction
                {
                    Amount = 50534,
                    Category = "Shopping",
                    Date = "2024-11-19T19:58:27.944Z",
                    Description = "transport desc",
                    Id = 4
                },
                new Transaction
                {
                    Amount = -24534,
                    Category = "Transport",
                    Date = "2024-12-19T19:58:19.162Z",
                    Description = "transport desc",
                    Id = 5
                },
                new Transaction
                {
                    Amount = -34534,
                    Category = "Shopping",
                    Date = "2024-12-19T19:58:27.944Z",
                    Description = "Shopping desc",
                    Id = 6
                },
                new Transaction
                {
                    Amount = 44534,
                    Category = "Transport",
                    Date = "2024-12-19T19:58:19.162Z",
                    Description = "transport desc",
                    Id = 7
                },
                new Transaction
                {
                    Amount = 54534,
                    Category = "Shopping",
                    Date = "2024-12-19T19:58:27.944Z",
                    Description = "transport desc",
                    Id = 8
                }
            );
            context.SaveChanges();
        }
    }
}
