using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Infrastructure.Domain.Options;

namespace VC.Admin.Components
{
	[ViewComponent(Name = "Assets")]
	public class AssetsViewComponent : ViewComponent
	{
	    private readonly IFrontEndAssetManager _frontEndAssetManager;
	    private readonly AppOptions appOptions;
		private readonly IUrlHelper urlHelper;

		public AssetsViewComponent(IUrlHelperFactory urlHelper, IOptions<AppOptions> appOptionsAccessor, IFrontEndAssetManager frontEndAssetManager, IActionContextAccessor actionContextAccessor)
		{
		    _frontEndAssetManager = frontEndAssetManager;

			if (appOptionsAccessor == null)
			{
				throw new ArgumentNullException(nameof(appOptionsAccessor));
			}
			this.urlHelper = urlHelper.GetUrlHelper(actionContextAccessor.ActionContext);
			this.appOptions = appOptionsAccessor.Value;
		}

		public IViewComponentResult Invoke(string assetType)
		{
			string viewName;
			IList<string> filePaths = new List<string>();

			var versionQueryString = appOptions.Versioning.EnableStaticContentVersioning ? $"?v={appOptions.Versioning.BuildNumber}" : string.Empty;
            if (assetType.Equals("scripts", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Scripts";
				var assetInfo = _frontEndAssetManager.GetScripts();
				if (appOptions.EnableBundlingAndMinification)
				{
					filePaths.Add(urlHelper.Content(
					    $"~/{assetInfo.MinifiedFileName}.min.js{versionQueryString}"));
				}
				else
				{
					foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(urlHelper.Content($"~/{assetFileInfo}{versionQueryString}"));
					}
				}
			}
			else if (assetType.Equals("styles", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Styles";
				var assetInfo = _frontEndAssetManager.GetStyles();
				if (appOptions.EnableBundlingAndMinification)
				{
					filePaths.Add(urlHelper.Content(
					    $"~/{assetInfo.MinifiedFileName}.min.css{versionQueryString}"));
				}
				else
				{
					foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(urlHelper.Content($"~/{assetFileInfo}{versionQueryString}"));
					}
				}
			}
            else if (assetType.Equals("styles-order-invoice", StringComparison.OrdinalIgnoreCase))
            {
                viewName = "Styles";
                var assetInfo = _frontEndAssetManager.GetOrderInvoiceStyles();
                if (appOptions.EnableBundlingAndMinification)
                {
                    filePaths.Add(urlHelper.Content(
                        $"~/{assetInfo.MinifiedFileName}.min.css{versionQueryString}"));
                }
                else
                {
                    foreach (var assetFileInfo in assetInfo.Files)
                    {
                        filePaths.Add(urlHelper.Content($"~/{assetFileInfo}{versionQueryString}"));
                    }
                }
            }
            else if (assetType.Equals("styles-report", StringComparison.OrdinalIgnoreCase))
            {
                viewName = "Styles";
                var assetInfo = _frontEndAssetManager.GetReportStyles();
                if (appOptions.EnableBundlingAndMinification)
                {
                    filePaths.Add(urlHelper.Content(
                        $"~/{assetInfo.MinifiedFileName}.min.css{versionQueryString}"));
                }
                else
                {
                    foreach (var assetFileInfo in assetInfo.Files)
                    {
                        filePaths.Add(urlHelper.Content($"~/{assetFileInfo}{versionQueryString}"));
                    }
                }
            }
            else
			{
				throw new InvalidOperationException($"Asset type '{assetType}' is not recognized.");
			}

			return View(viewName, filePaths);
		}
	}
}