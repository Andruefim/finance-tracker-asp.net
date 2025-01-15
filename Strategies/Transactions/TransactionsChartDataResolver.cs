using System.Collections.Concurrent;
using System.Reflection;
using AngularWithASP.Server.Data;
using AngularWithASP.Server.Interfaces;

namespace AngularWithASP.Server.Strategies.Transactions;

public class TransactionsChartDataResolver
{
    private readonly ApplicationDbContext _context;

    private ConcurrentDictionary<TransactionDataType, ITransactionsChartDataStrategy> _strategies = new();

    public TransactionsChartDataResolver(ApplicationDbContext context)
    {
        _context = context;

        Init();
    }

    private void Init()
    {
        var strategyTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(ITransactionsChartDataStrategy).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        foreach (var type in strategyTypes)
        {
            var strategy = (ITransactionsChartDataStrategy)Activator.CreateInstance(type, _context);

            if (strategy == null)
            {
                continue;
            }

            _strategies.TryAdd(strategy.Type, strategy);
        }
    }

    public ITransactionsChartDataStrategy GetStrategy(TransactionDataType type)
    {
        if (_strategies.ContainsKey(type))
            return _strategies[type];

        throw new NotImplementedException();
    }
}
