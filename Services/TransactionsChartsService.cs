using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Models;
using AngularWithASP.Server.Data;
using System.Runtime.CompilerServices;
using Microsoft.OpenApi.Any;
using AngularWithASP.Server.Auth;

namespace AngularWithASP.Server.Services;

public interface ITransactionsChartsService
{
    Task<List<TransactionsChart>> GetTransactionsChartAsync(string userId);
    Task<List<ExpensesChart>> GetExpensesChartAsync(string userId);
}

public class TransactionsChartsService : ITransactionsChartsService
{
    private readonly ApplicationDbContext _context;

    public TransactionsChartsService(ApplicationDbContext context) { 
        _context = context;
    }

    public async Task<List<TransactionsChart>> GetTransactionsChartAsync(string userId)
    { 
        var incomeTransactionsData = await _context.Transactions
            .Where(t => t.Amount >= 0 && t.UserId == userId)
            .Select(t => new TransactionsChartData { date = t.Date, amount = t.Amount })
            .ToListAsync();

        var expensesTransactionsData = await _context.Transactions
            .Where(t => t.Amount <= 0 && t.UserId == userId)
            .Select(t => new TransactionsChartData { date = t.Date, amount = Math.Abs(t.Amount) })
            .ToListAsync();


        return new List<TransactionsChart>
        {
            new TransactionsChart { Type = "Income", Data = incomeTransactionsData },
            new TransactionsChart { Type = "Expenses", Data = expensesTransactionsData },
        };
    }

    public async Task<List<ExpensesChart>> GetExpensesChartAsync(string userId)
    {
        var expensesChartData = new List<ExpensesChart>();

        if (_context.Transactions == null)
        {
            return expensesChartData;
        }

        // Use LINQ to get ExpensesChart data list
        var categories = await _context.Transactions
                            .Where(t => t.UserId == userId)
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
                                .Where(t => t.UserId == userId)
                                .Where(t => t.Category == category && t.Amount < 0)
                                .Sum(t => Math.Abs(t.Amount))
                    }
                );
        }

        return expensesChartData;
    }
}
