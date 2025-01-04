using AngularWithASP.Server.Data;
using AngularWithASP.Server.Extensions;
using AngularWithASP.Server.Interfaces;
using AngularWithASP.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularWithASP.Server.Strategies;

public class CreateIncomeChartDataStrategy : ITransactionsChartDataStrategy
{
    private readonly ApplicationDbContext _context;

    public TransactionDataType Type => TransactionDataType.Income;

    public CreateIncomeChartDataStrategy(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransactionsChartData>> CreateTransactionsChartData(string userId)
    {
        var transactionsData = await _context.Transactions
            .ByUserId(userId)
            .Where(t => t.Amount >= 0)
            .Select(t => new TransactionsChartData { date = t.Date, amount = t.Amount })
            .ToListAsync();

        return transactionsData;
    }
}

