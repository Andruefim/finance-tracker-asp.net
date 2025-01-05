using System.Linq.Expressions;
using AngularWithASP.Server.Models;

namespace AngularWithASP.Server.Expressions;

public static class TransactionExpressions
{
    public static Expression<Func<Transaction, long>> ToAbsoluteAmount()
    { 
        return t => Math.Abs(t.Amount);
    }

    public static Expression<Func<Transaction, bool>> IsExpenseTransaction()
    {
        return t => t.Amount < 0;
    }

    public static Expression<Func<Transaction, bool>> IsIncomeTransaction()
    {
        return t => t.Amount >= 0;
    }
}
