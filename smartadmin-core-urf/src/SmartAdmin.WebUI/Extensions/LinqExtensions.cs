using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartAdmin.WebUI.Extensions
{
  public static class LinqExtensions
  {
    private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel)
    {
      ParameterExpression param = Expression.Parameter(typeof(T), string.Empty); // I don't care about some naming
      MemberExpression property = Expression.PropertyOrField(param, propertyName);
      LambdaExpression sort = Expression.Lambda(property, param);

      MethodCallExpression call = Expression.Call(
          typeof(Queryable),
          (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
          new[] { typeof(T), property.Type },
          source.Expression,
          Expression.Quote(sort));

      return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
    {
      return OrderingHelper(source, propertyName, false, false);
    }
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, string order)
    {
      if (order.ToLower() == "desc")
        return OrderingHelper(source, propertyName, true, false);
      else
        return OrderingHelper(source, propertyName, false, false);
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
    {
      return OrderingHelper(source, propertyName, true, false);
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
    {
      return OrderingHelper(source, propertyName, false, true);
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
    {
      return OrderingHelper(source, propertyName, true, true);
    }

    public static IQueryable<T> OrderByName<T>(this IQueryable<T> q, string SortField, bool Ascending = true)
    {
      if (SortField.IndexOf("DESC") > 0)
      {
        SortField = SortField.Split(new char[] { ' ' })[0];
        Ascending = false;
      }
      var param = Expression.Parameter(typeof(T), "p");
      var prop = Expression.Property(param, SortField);
      var exp = Expression.Lambda(prop, param);
      string method = Ascending ? "OrderBy" : "OrderByDescending";
      Type[] types = new Type[] { q.ElementType, exp.Body.Type };
      var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
      return q.Provider.CreateQuery<T>(mce);
    }
    public static IQueryable<T> OrderByName<T>(this IQueryable<T> q, string SortField, string order = "desc")
    {
      if (order.ToLower() == "desc")
      {
        return OrderByName(q, SortField, false);
      }
      else
      {
        return OrderByName(q, SortField, true);
      }
    }
  }
}
