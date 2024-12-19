using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Models;

namespace AngularWithASP.Server.Controllers
{
    //[Route("api/transactions/charts/")]
    [ApiController]
    public class TransactionsChartsController : ControllerBase
    {
        private readonly TransactionsContext _context;

        public TransactionsChartsController(TransactionsContext context) {
            _context = context;
        }

        [HttpGet("api/transactions/charts/expenses-chart")]
        public async Task<ActionResult<IEnumerable<ExpensesChart>>> GetExpensesChart()
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set TransactionsContext.Transactions is null.");
            }

            // Use LINQ to get ExpensesChart data list
            var categories = await _context.Transactions
                                .Select(t => t.Category)
                                .Distinct()
                                .ToListAsync();

            var expensesChartData = new List<ExpensesChart>();

            foreach (var category in categories) {
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

            return expensesChartData.ToList();
        }
    }
}
