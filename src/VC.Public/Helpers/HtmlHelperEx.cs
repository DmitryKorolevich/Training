using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using VitalChoice.Ecommerce.Domain.Helpers;

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

        public static DateTime ConvertToPst(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local, TimeZoneHelper.PstTimeZoneInfo);
        }
    }
}
