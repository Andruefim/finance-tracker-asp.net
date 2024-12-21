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

namespace AngularWithASP.Server.Services;

public interface ITransactionsChartsService
{
    Task<List<TransactionsChart>> GetTransactionsChartAsync();
    Task<List<ExpensesChart>> GetExpensesChartAsync();
}

public class TransactionsChartsService : ITransactionsChartsService
{
    private readonly TransactionsContext _context;

    public TransactionsChartsService(TransactionsContext context) { 
        _context = context;
    }

    public async Task<List<TransactionsChart>> GetTransactionsChartAsync()
    { 
        var transactionsChart = new List<TransactionsChart>();

        if (_context.Transactions == null)
        { 
            return transactionsChart;
        }

        // Use LINQ to get TransactionsChart data list
        var transactions = from t in _context.Transactions
                           select t;

        var incomeTransactionsData = await _context.Transactions
            .Where(t => t.Amount >= 0)
            .Select(t => new TransactionsChartData { date = t.Date, amount = t.Amount })
            .ToListAsync();

        var expensesTransactionsData = await _context.Transactions
            .Where(t => t.Amount <= 0)
            .Select(t => new TransactionsChartData { date = t.Date, amount = Math.Abs(t.Amount) })
            .ToListAsync();

        transactionsChart.Add(
            new TransactionsChart
            {
                Type = "Income",
                Data = incomeTransactionsData
            }
        );

        transactionsChart.Add(
            new TransactionsChart
            { 
                Type = "Expenses",
                Data = expensesTransactionsData
            }
        );

        return transactionsChart;
    }

    public async Task<List<ExpensesChart>> GetExpensesChartAsync()
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
