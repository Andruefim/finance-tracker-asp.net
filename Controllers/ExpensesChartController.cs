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
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesChartController : ControllerBase
    {
        private readonly TransactionsContext _transactionsContext;

        public ExpensesChartController(TransactionsContext transactionsContext) {
            _transactionsContext = transactionsContext;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ExpensesChart>>> GetExpensesChart() { 
        //    return await _transactionsContext.Transactions
        //}
    }
}
