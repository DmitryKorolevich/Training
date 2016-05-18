using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Core.Infrastructure
{
    public class CustomUrlHelperFactory : IUrlHelperFactory
    {
        private readonly IOptions<AppOptions> _appOptions;

        public CustomUrlHelperFactory(IOptions<AppOptions> appOptions)
        {
            _appOptions = appOptions;
        }

        public IUrlHelper GetUrlHelper(ActionContext context)
        {
            HttpContext httpContext = context.HttpContext;
            if (httpContext?.Items == null)
            {
                throw new ArgumentNullException(nameof(httpContext.Items));
            }
            IUrlHelper urlHelper = httpContext.Items[typeof(IUrlHelper)] as IUrlHelper;

            if (urlHelper == null)
            {
                urlHelper = new CustomUrlHelper(context, _appOptions);
                httpContext.Items[typeof(IUrlHelper)] = urlHelper;
            }
            return urlHelper;
        }
    }

	public class CustomUrlHelper : UrlHelper
	{
		private readonly IOptions<AppOptions> _appOptions;

		public CustomUrlHelper(ActionContext context,
							   IOptions<AppOptions> appOptions) : base(context)
		{
			this._appOptions = appOptions;
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
			if (!string.IsNullOrEmpty(url) && _appOptions.Value.GenerateLowercaseUrls)
			{
				return url.ToLowerInvariant();
			}

			return url;
		}
	}
}