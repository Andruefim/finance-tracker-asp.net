using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Models;
using AngularWithASP.Server.Data;

namespace AngularWithASP.Server.Services;

public interface ITransactionsChartsService
{
    Task<List<ExpensesChart>> GetExpensesChartData();
}

public class TransactionsChartsService : ITransactionsChartsService
{
    private readonly TransactionsContext _context;

    public TransactionsChartsService(TransactionsContext context) { 
        _context = context;
    }
    public async Task<List<ExpensesChart>> GetExpensesChartData()
    {
        var expensesChartData = new List<ExpensesChart>();

        if (_context.Transactions == null)
        {
            return expensesChartData;
        }

        // Use LINQ to get ExpensesChart data list
        var categories = await _context.Transactions
                            .Select(t => t.Category)
                            .Distinct()
                            .ToListAsync();


        foreach (var category in categories)
        {
            expensesChartData
                .Add(
                    new ExpensesChart
                    {
                        Category = category,
                        Amount = _context.Transactions
                                .Where(t => t.Category == category && t.Amount < 0)
                                .Sum(t => Math.Abs(t.Amount))
                    }
                );
        }

        return expensesChartData;
    }
}
