using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Text.Json;


namespace URF.Core.EF
{
  internal class filteritem
  {
    public string field { get; set; }
    public string op { get; set; }
    public string value { get; set; }
  }
  public static class PredicateBuilder
  {

    public static Expression<Func<T, bool>> FromFilter<T>(string filtergroup) {
      Expression<Func<T, bool>> any = x => true;
      if (!string.IsNullOrEmpty(filtergroup))
      {
        var filters = JsonSerializer.Deserialize<filteritem[]>(filtergroup);

          foreach (var filter in filters)
          {
            if (Enum.TryParse(filter.op, out OperationExpression op) && !string.IsNullOrEmpty(filter.value))
            {
              var expression = GetCriteriaWhere<T>(filter.field, op, filter.value);
              any = any.And(expression);
            }
          }
      }

      return any;
    }

    #region -- Public methods --
    public static Expression<Func<T, bool>> GetCriteriaWhere<T>(Expression<Func<T, object>> e, OperationExpression selectedOperator, object fieldValue)
    {
      var name = GetOperand<T>(e);
      return GetCriteriaWhere<T>(name, selectedOperator, fieldValue);
    }

    public static Expression<Func<T, bool>> GetCriteriaWhere<T, T2>(Expression<Func<T, object>> e, OperationExpression selectedOperator, object fieldValue)
    {
      var name = GetOperand<T>(e);
      return GetCriteriaWhere<T, T2>(name, selectedOperator, fieldValue);
    }

    public static Expression<Func<T, bool>> GetCriteriaWhere<T>(string fieldName, OperationExpression selectedOperator, object fieldValue)
    {
      var props = TypeDescriptor.GetProperties(typeof(T));
      var prop = GetProperty(props, fieldName, true);
      var parameter = Expression.Parameter(typeof(T));
      var expressionParameter = GetMemberExpression<T>(parameter, fieldName);
      if (prop != null && fieldValue != null)
      {
       
        BinaryExpression body = null;
        switch (selectedOperator)
        {
          case OperationExpression.equal:
            body = Expression.Equal(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
          case OperationExpression.notequal:
            body = Expression.NotEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
          case OperationExpression.less:
            body = Expression.LessThan(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
          case OperationExpression.lessorequal:
            body = Expression.LessThanOrEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
          case OperationExpression.greater:
            body = Expression.GreaterThan(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
          case OperationExpression.greaterorequal:
            body = Expression.GreaterThanOrEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(body, parameter);
          case OperationExpression.contains:
            var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var bodyLike = Expression.Call(expressionParameter, contains, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(bodyLike, parameter);
          case OperationExpression.endwith:
            var endswith = typeof(string).GetMethod("EndsWith",new[] { typeof(string) });
            var bodyendwith = Expression.Call(expressionParameter, endswith, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(bodyendwith, parameter);
          case OperationExpression.beginwith:
            var startswith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            var bodystartswith = Expression.Call(expressionParameter, startswith, Expression.Constant(Convert.ChangeType(fieldValue, prop.PropertyType), prop.PropertyType));
            return Expression.Lambda<Func<T, bool>>(bodystartswith, parameter);
          case OperationExpression.includes:
            return Includes<T>(fieldValue, parameter, expressionParameter, prop.PropertyType);
          case OperationExpression.between:
            return Between<T>(fieldValue, parameter, expressionParameter, prop.PropertyType);
          default:
            throw new Exception("Not implement Operation");
        }
      }
      else
      {
        Expression<Func<T, bool>> filter = x => true;
        return filter;
      }
    }

    public static Expression<Func<T, bool>> GetCriteriaWhere<T, T2>(string fieldName, OperationExpression selectedOperator, object fieldValue)
    {


      var props = TypeDescriptor.GetProperties(typeof(T));
      var prop = GetProperty(props, fieldName, true);

      var parameter = Expression.Parameter(typeof(T));
      var expressionParameter = GetMemberExpression<T>(parameter, fieldName);

      if (prop != null && fieldValue != null)
      {
        switch (selectedOperator)
        {
          case OperationExpression.any:
            return Any<T, T2>(fieldValue, parameter, expressionParameter);

          default:
            throw new Exception("Not implement Operation");
        }
      }
      else
      {
        Expression<Func<T, bool>> filter = x => true;
        return filter;
      }
    }



    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr, Expression<Func<T, bool>> or)
    {
      if (expr == null)
      {
        return or;
      }

      return Expression.Lambda<Func<T, bool>>(Expression.OrElse(new SwapVisitor(expr.Parameters[0], or.Parameters[0]).Visit(expr.Body), or.Body), or.Parameters);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr, Expression<Func<T, bool>> and)
    {
      if (expr == null)
      {
        return and;
      }

      return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(new SwapVisitor(expr.Parameters[0], and.Parameters[0]).Visit(expr.Body), and.Body), and.Parameters);
    }

    #endregion
    #region -- Private methods --

    private static string GetOperand<T>(Expression<Func<T, object>> exp)
    {
      if (!( exp.Body is MemberExpression body ))
      {
        var ubody = (UnaryExpression)exp.Body;
        body = ubody.Operand as MemberExpression;
      }

      var operand = body.ToString();

      return operand.Substring(2);

    }

    private static MemberExpression GetMemberExpression<T>(ParameterExpression parameter, string propName)
    {
      if (string.IsNullOrEmpty(propName))
      {
        return null;
      }

      var propertiesName = propName.Split('.');
      if (propertiesName.Count() == 2)
      {
        return Expression.Property(Expression.Property(parameter, propertiesName[0]), propertiesName[1]);
      }

      return Expression.Property(parameter, propName);
    }

    private static Expression<Func<T, bool>> Includes<T>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression ,Type type)
    {
      var safetype= Nullable.GetUnderlyingType(type) ?? type;

      switch (safetype.Name.ToLower())
      {
        case  "string":
          var strlist = (IEnumerable<string>)fieldValue;
          if (strlist == null || strlist.Count() == 0)
          {
            return x => true;
          }
          var strmethod = typeof(List<string>).GetMethod("Contains", new Type[] { typeof(string) });
          var strcallexp = Expression.Call(Expression.Constant(strlist.ToList()), strmethod, memberExpression);
          return Expression.Lambda<Func<T, bool>>(strcallexp, parameterExpression);
        case "int32":
          var intlist = (IEnumerable<int>)fieldValue;
          if (intlist == null || intlist.Count() == 0)
          {
            return x => true;
          }
          var intmethod = typeof(List<int>).GetMethod("Contains", new Type[] { typeof(int) });
          var intcallexp = Expression.Call(Expression.Constant(intlist.ToList()), intmethod, memberExpression);
          return Expression.Lambda<Func<T, bool>>(intcallexp, parameterExpression);
        case "float":
          var floatlist = (IEnumerable<float>)fieldValue;
          if (floatlist == null || floatlist.Count() == 0)
          {
            return x => true;
          }
          var floatmethod = typeof(List<int>).GetMethod("Contains", new Type[] { typeof(int) });
          var floatcallexp = Expression.Call(Expression.Constant(floatlist.ToList()), floatmethod, memberExpression);
          return Expression.Lambda<Func<T, bool>>(floatcallexp, parameterExpression);
        default:
          return x => true;
      }
      
    }
    private static Expression<Func<T, bool>> Between<T>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression, Type type)
    {
      
      var safetype = Nullable.GetUnderlyingType(type) ?? type;
      switch (safetype.Name.ToLower())
      {
        case "datetime":
          var datearray = ( (string)fieldValue ).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
          var start = Convert.ToDateTime(datearray[0] + " 00:00:00", CultureInfo.CurrentCulture);
          var end = Convert.ToDateTime(datearray[1] + " 23:59:59", CultureInfo.CurrentCulture);
          var greater = Expression.GreaterThan(memberExpression, Expression.Constant(start, type));
          var less = Expression.LessThan(memberExpression, Expression.Constant(end, type));
          return Expression.Lambda<Func<T, bool>>(greater, parameterExpression)
            .And(Expression.Lambda<Func<T, bool>>(less, parameterExpression));
        case "int":
        case "int32":
          var intarray = ( (string)fieldValue ).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
          var min = Convert.ToInt32(intarray[0] , CultureInfo.CurrentCulture);
          var max = Convert.ToInt32(intarray[1], CultureInfo.CurrentCulture);
          var maxthen = Expression.GreaterThan(memberExpression, Expression.Constant(min, type));
          var minthen = Expression.LessThan(memberExpression, Expression.Constant(max, type));
          return Expression.Lambda<Func<T, bool>>(maxthen, parameterExpression)
            .And(Expression.Lambda<Func<T, bool>>(minthen, parameterExpression));
        case "decimal":
          var decarray = ( (string)fieldValue ).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
          var dmin = Convert.ToDecimal(decarray[0], CultureInfo.CurrentCulture);
          var dmax = Convert.ToDecimal(decarray[1], CultureInfo.CurrentCulture);
          var dmaxthen = Expression.GreaterThan(memberExpression, Expression.Constant(dmin, type));
          var dminthen = Expression.LessThan(memberExpression, Expression.Constant(dmax, type));
          return Expression.Lambda<Func<T, bool>>(dmaxthen, parameterExpression)
            .And(Expression.Lambda<Func<T, bool>>(dminthen, parameterExpression));
        case "float":
          var farray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
          var fmin = Convert.ToDecimal(farray[0], CultureInfo.CurrentCulture);
          var fmax = Convert.ToDecimal(farray[1], CultureInfo.CurrentCulture);
          var fmaxthen = Expression.GreaterThan(memberExpression, Expression.Constant(fmin, type));
          var fminthen = Expression.LessThan(memberExpression, Expression.Constant(fmax, type));
          return Expression.Lambda<Func<T, bool>>(fmaxthen, parameterExpression)
            .And(Expression.Lambda<Func<T, bool>>(fminthen, parameterExpression));
        case "string":
          var strarray = ( (string)fieldValue ).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
          var smin = strarray[0];
          var smax = strarray[1];
        
          var strmethod = typeof(string).GetMethod("Contains");
          var mm = Expression.Call(memberExpression, strmethod, Expression.Constant(smin, type));
          var nn = Expression.Call(memberExpression, strmethod, Expression.Constant(smax, type));


          return Expression.Lambda<Func<T, bool>>(mm, parameterExpression)
            .Or(Expression.Lambda<Func<T, bool>>(nn, parameterExpression));
        default:
          return x => true;
      }

    }



    private static Expression<Func<T, bool>> Any<T, T2>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression)
    {
      var lambda = (Expression<Func<T2, bool>>)fieldValue;
      var anyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
      .First(m => m.Name == "Any" && m.GetParameters().Count() == 2).MakeGenericMethod(typeof(T2));

      var body = Expression.Call(anyMethod, memberExpression, lambda);

      return Expression.Lambda<Func<T, bool>>(body, parameterExpression);
    }

    private static PropertyDescriptor GetProperty(PropertyDescriptorCollection props, string fieldName, bool ignoreCase)
    {
      if (!fieldName.Contains('.'))
      {
        return props.Find(fieldName, ignoreCase);
      }

      var fieldNameProperty = fieldName.Split('.');
      return props.Find(fieldNameProperty[0], ignoreCase).GetChildProperties().Find(fieldNameProperty[1], ignoreCase);

    }
    #endregion
  }

  internal class SwapVisitor : ExpressionVisitor
  {
    private readonly Expression from, to;
    public SwapVisitor(Expression from, Expression to)
    {
      this.from = from;
      this.to = to;
    }
    public override Expression Visit(Expression node) => node == from ? to : base.Visit(node);
  }
  public enum OperationExpression
  {
    equal,
    notequal,
    less,
    lessorequal,
    greater,
    greaterorequal,
    contains,
    beginwith,
    endwith,
    includes,
    between,
    any
  }
}
