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
    [Route("api/dashboard/expenses-chart")]
    [ApiController]
    public class ExpensesChartController : ControllerBase
    {
        private readonly TransactionsContext _context;

        public ExpensesChartController(TransactionsContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpensesChart>>> GetExpensesChart()
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set TransactionsContext.Transactions is null.");
            }

            // Use LINQ to get ExpensesChart data list
            IQueryable<string> categoryQuery = from t in _context.Transactions
                                               select t.Category;
            var categories = await categoryQuery.Distinct().ToListAsync();
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
