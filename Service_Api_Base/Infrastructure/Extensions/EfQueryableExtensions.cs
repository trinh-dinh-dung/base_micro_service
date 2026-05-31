using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Extensions
{
    public static class EfQueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string columnName, bool isAscending = true)
        {
            if (string.IsNullOrEmpty(columnName))
                return source;

            var parameter = Expression.Parameter(source.ElementType, string.Empty);
            var property = Expression.Property(parameter, columnName);
            var lambda = Expression.Lambda(property, parameter);
            var methodName = isAscending ? "OrderBy" : "OrderByDescending";

            var methodCallExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { source.ElementType, property.Type },
                source.Expression,
                Expression.Quote(lambda));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }
    }
}
