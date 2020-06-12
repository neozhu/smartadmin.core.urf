using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SmartAdmin.WebUI.Extensions
{
  public static class HtmlHelperExtensions
  {
    public static IHtmlContent DescriptionFor<TModel, TValue>(this IHtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
    {
      if (self == null) throw new ArgumentNullException(nameof(self));
      if (expression == null) throw new ArgumentNullException(nameof(expression));
      var modelExpressionProvider = (ModelExpressionProvider)self.ViewContext.HttpContext.RequestServices.GetService(typeof(IModelExpressionProvider));
      var modelExplorer = modelExpressionProvider.CreateModelExpression(self.ViewData, expression);
      if (modelExplorer == null) throw new InvalidOperationException($"Failed to get model explorer for {modelExpressionProvider.GetExpressionText(expression)}");
      return new HtmlString(modelExplorer.Metadata.Description);
    }
  }
}
