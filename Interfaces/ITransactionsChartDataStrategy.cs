using AngularWithASP.Server.Models;

namespace AngularWithASP.Server.Interfaces;

public enum TransactionDataType
{
    Income,
    Expenses
}

public interface ITransactionsChartDataStrategy
{
    TransactionDataType Type { get; }
    Task<List<TransactionsChartData>> CreateTransactionsChartData(string userId);
}
