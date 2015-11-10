using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Html.Abstractions;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace VC.Public.Helpers
{
	public static class HtmlHelper
	{
		public static IHtmlContent DisplayForEx<TModel, TResult>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TResult>> expression)
		{
			{
				var modelExporer = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData,
					htmlHelper.MetadataProvider);

				var value = modelExporer.Model;
				if (value == null)
				{
					MemberExpression memberExpression;
					var expressionBody = expression.Body;
					if (expressionBody.NodeType == ExpressionType.Convert)
					{
						memberExpression = ((UnaryExpression) expressionBody).Operand as MemberExpression;
					}
					else
					{
						memberExpression = expressionBody as MemberExpression;
					}

					ModelStateEntry modelState;
					if (memberExpression != null && (modelState = htmlHelper.ViewContext.ModelState[memberExpression.Member.Name]) != null)
					{
						value = modelState.RawValue;
					}
				}

				return new HtmlString(value?.ToString() ?? string.Empty);
			}
		}
	}
}
