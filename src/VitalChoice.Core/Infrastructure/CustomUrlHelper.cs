﻿using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Core.Infrastructure
{
	/// <summary>
	/// Following are some of the scenarios exercised here:
	/// 1. Based on configuration, generate Content urls pointing to local or a CDN server
	/// 2. Based on configuration, generate lower case urls
	/// </summary>
	/// <remarks>
	/// ref: https://github.com/aspnet/Mvc/blob/dev/test/WebSites/UrlHelperWebSite/CustomUrlHelper.cs
	/// </remarks>
	public class CustomUrlHelper : UrlHelper
	{
		private readonly IOptions<AppOptions> appOptions;
		private readonly HttpContext httpContext;

		public CustomUrlHelper(IActionContextAccessor contextAccessor,
							   IActionSelector actionSelector,
							   IOptions<AppOptions> appOptions) : base(contextAccessor.ActionContext)
		{
			this.appOptions = appOptions;
			this.httpContext = contextAccessor.ActionContext.HttpContext;
		}

		/// <summary>
		/// Depending on config data, generates an absolute url pointing to a CDN server
		/// or falls back to the default behavior
		/// </summary>
		/// <param name="contentPath"></param>
		/// <returns></returns>
		public override string Content(string contentPath)
		{
			return ConvertToLowercaseUrl(base.Content(contentPath));
		}

		public override string RouteUrl(UrlRouteContext context)
		{
			return ConvertToLowercaseUrl(base.RouteUrl(context));
		}

		public override string Action(UrlActionContext context)
		{
			return ConvertToLowercaseUrl(base.Action(context));
		}

		private string ConvertToLowercaseUrl(string url)
		{
			if (!string.IsNullOrEmpty(url)
				&& appOptions.Value.GenerateLowercaseUrls)
			{
				return url.ToLowerInvariant();
			}

			return url;
		}
	}
}