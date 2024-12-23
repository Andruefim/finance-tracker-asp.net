﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Models;
using AngularWithASP.Server.Data;
using AngularWithASP.Server.Services;

namespace AngularWithASP.Server.Controllers
{
    //[Route("api/transactions/charts/")]
    [ApiController]
    public class TransactionsChartsController : ControllerBase
    {
        private readonly TransactionsContext _context;
        private readonly ITransactionsChartsService _transactionsChartsService;

        public TransactionsChartsController(
            TransactionsContext context,
            ITransactionsChartsService transactionsChartsService
        ) {
            _context = context;
            _transactionsChartsService = transactionsChartsService;
        }

        [HttpGet("/api/transactions/charts/transactions-chart")]
        public async Task<ActionResult<IEnumerable<TransactionsChart>>> GetTransactionsChart()
        {
            var transactionsChartData = await _transactionsChartsService.GetTransactionsChartAsync();

            if (transactionsChartData.Count == 0)
            {
                return NotFound("No transactions found");
            }

            return Ok(transactionsChartData);
        }

        [HttpGet("api/transactions/charts/expenses-chart")]
        public async Task<ActionResult<IEnumerable<ExpensesChart>>> GetExpensesChart()
        {
            var expensesChartData = await _transactionsChartsService.GetExpensesChartAsync();

            if (expensesChartData.Count == 0)
            {
                return NotFound("No transactions found");
            }

            return Ok(expensesChartData);
        }
    }
}
