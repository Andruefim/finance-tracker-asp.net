using System.Collections.Concurrent;
using System.Reflection;
using AngularWithASP.Server.Data;
using AngularWithASP.Server.Interfaces;

namespace AngularWithASP.Server.Strategies.Categories;

public class CategoriesDataResolver
{
    private readonly ApplicationDbContext _context;

    private ConcurrentDictionary<CategoriesDataType, ICategoriesDataStrategy> _strategies = new();

    public CategoriesDataResolver(ApplicationDbContext context)
    { 
        _context = context;

        Init();
    }

    private void Init()
    {
        var strategyTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(ICategoriesDataStrategy).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

        foreach (var type in strategyTypes)
        {
            var strategy = (ICategoriesDataStrategy)Activator.CreateInstance(type, _context);

            if (strategy == null)
            {
                continue;
            }

            _strategies.TryAdd(strategy.Type, strategy);
        }
    }

    public ICategoriesDataStrategy GetStrategy(CategoriesDataType type)
    { 
        if (_strategies.ContainsKey(type))
            return _strategies[type];

        throw new InvalidOperationException();
    }
}
