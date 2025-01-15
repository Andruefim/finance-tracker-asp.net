using AngularWithASP.Server.Data;
using AngularWithASP.Server.Expressions;
using AngularWithASP.Server.Extensions;
using AngularWithASP.Server.Interfaces;
using AngularWithASP.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularWithASP.Server.Strategies.Categories;

public class CreateExpensesCategoriesDataStrategy : ICategoriesDataStrategy
{
    private readonly ApplicationDbContext _context;

    public CategoriesDataType Type => CategoriesDataType.Expenses;

    public CreateExpensesCategoriesDataStrategy(ApplicationDbContext context)
    { 
        _context = context;
    }

    public async Task<List<Category>> CreateCategoriesData(string userId)
    { 
        return await _context.Categories
            .ByUserId(userId)
            .Where(c => c.Type == nameof(CategoriesDataType.Expenses))
            .ToListAsync();
    }
}
