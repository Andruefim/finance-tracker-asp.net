using AngularWithASP.Server.Models;
using System.Linq;
using AngularWithASP.Server.Interfaces;

namespace AngularWithASP.Server.Extensions;

public static class UserExtensions
{
    public static IQueryable<T> ByUserId<T>(this IQueryable<T> query, string userId) where T : IUserOwned
    {
        return query.Where(c => c.UserId == userId);
    }
}
