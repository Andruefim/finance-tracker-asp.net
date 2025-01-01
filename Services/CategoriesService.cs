using Microsoft.EntityFrameworkCore;
using AngularWithASP.Server.Models;
using AngularWithASP.Server.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Linq.Expressions;
using AngularWithASP.Server.Extensions;

namespace AngularWithASP.Server.Services;


public interface ICategoriesService
{ 
    Task<IEnumerable<Category>> GetCategoriesAsync(string userId);
    Task<Category> UpdateCategoryAsync(long id, Category category);
    Task<Category> AddCategoryAsync(string userId, Category category);
    Task<bool> DeleteCategoryAsync(long id);
}


public class CategoriesService : ICategoriesService
{
    private readonly ApplicationDbContext _context;

    public CategoriesService(ApplicationDbContext context) {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(string userId)
    {
        return await _context.Categories
            .ByUserId(userId)
            .ToListAsync();
    }

    public async Task<Category> UpdateCategoryAsync(long id, Category category)
    {
        if (id != category.Id)
        {
            return null;
        }

        _context.Entry(category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
            {
                return null;
            }
            else
            {
                throw;
            }
        }

        return category;
    }

    public async Task<Category> AddCategoryAsync(string userId, Category category) { 
        category.UserId = userId;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(long id) { 
        var category = await _context.Categories.FindAsync(id);
        if (category == null) { 
            return false;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    private bool CategoryExists(long id) { 
        return _context.Categories.Any(c => c.Id == id);
    }
}
