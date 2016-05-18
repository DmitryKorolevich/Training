﻿using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Net.Http.Server;

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

        private static TimeZoneInfo _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        public static DateTime ConvertToPST(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, TimeZoneInfo.Local, _pstTimeZoneInfo);
        }
    }
}
