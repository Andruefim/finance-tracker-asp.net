using System.Linq.Expressions;
using AngularWithASP.Server.Models;

namespace AngularWithASP.Server.Expressions;

public static class TransactionExpressions
{
    public static Expression<Func<Transaction, long>> ToAbsoluteAmount()
    { 
        //return t => Math.Abs(t.Amount);

        var tParameter = Expression.Parameter(typeof(Transaction), "t");
        var amountProperty = Expression.Property(tParameter, nameof(Transaction.Amount));

        var absMethod = typeof(Math).GetMethod(nameof(Math.Abs), [typeof(long)]) 
            ?? throw new InvalidOperationException($"{nameof(Math.Abs)} not found!");

        var absoluteAmount = Expression.Call(absMethod, amountProperty);

        return Expression.Lambda<Func<Transaction, long>>(absoluteAmount, tParameter);
    }

    public static Expression<Func<Transaction, bool>> IsExpenseTransaction()
    {
        //return t => t.Amount < 0;
        var tParameter = Expression.Parameter(typeof(Transaction), "t");
        var amountProperty = Expression.Property(tParameter, nameof(Transaction.Amount));
        var negativeAmount = Expression.LessThan(amountProperty, Expression.Constant(0));

        return Expression.Lambda<Func<Transaction, bool>>(negativeAmount, tParameter);
    }

    public static Expression<Func<Transaction, bool>> IsIncomeTransaction()
    {
        //return t => t.Amount >= 0;
        var tParameter = Expression.Parameter(typeof(Transaction), "t");
        var amountProperty = Expression.Property(tParameter, nameof(Transaction.Amount));
        var positiveAmount = Expression.GreaterThanOrEqual(amountProperty, Expression.Constant(0));

        return Expression.Lambda<Func<Transaction, bool>>(positiveAmount, tParameter);

    }
}
