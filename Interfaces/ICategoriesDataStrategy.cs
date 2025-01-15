using AngularWithASP.Server.Models;

namespace AngularWithASP.Server.Interfaces;

public enum CategoriesDataType
{ 
    Income,
    Expenses
}

public interface ICategoriesDataStrategy
{
    CategoriesDataType Type { get; }
    Task<List<Category>> CreateCategoriesData(string userId);
}
