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
using System.Security.Claims;
using AngularWithASP.Server.Strategies;
using AngularWithASP.Server.Interfaces;
using AngularWithASP.Server.Extensions;
using AngularWithASP.Server.Expressions;

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
        var resolver = new TransactionsChartDataResolver(_context);

        return
        [
            new ()
            { 
                Type = nameof(TransactionDataType.Income), 
                Data = await resolver
                    .GetStrategy(TransactionDataType.Income)
                    .CreateTransactionsChartData(userId)
            },
            new ()
            { 
                Type = nameof(TransactionDataType.Expenses), 
                Data = await resolver
                    .GetStrategy(TransactionDataType.Expenses)
                    .CreateTransactionsChartData(userId)
            },
        ];
    }

    public async Task<List<ExpensesChart>> GetExpensesChartAsync(string userId)
    {
        if (_context.Transactions == null)
        {
            return [];
        }

        // Use LINQ to get ExpensesChart data list
        var expensesChartData = await _context.Transactions
                            .ByUserId(userId)
                            .Select(t => t.Category)
                            .Distinct()
                            .Select(category => new ExpensesChart
                            {
                                Category = category,
                                Amount = _context.Transactions
                                .ByUserId(userId)
                                .Where(t => t.Category == category)
                                .Where(TransactionExpressions.IsExpenseTransaction())
                                .Sum(TransactionExpressions.ToAbsoluteAmount())
                            })
                            .ToListAsync();

        return expensesChartData;
    }
}
