using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Infrastructure.Domain.Options;

namespace VC.Public.Components
{
	[ViewComponent(Name = "Assets")]
	public class AssetsViewComponent : ViewComponent
	{
		private readonly AppOptions _appOptions;
		private readonly IUrlHelper _urlHelper;
	    private static string _minifiedJsName;
	    private static string _minifiedCssName;

		public AssetsViewComponent(IUrlHelper urlHelper, IOptions<AppOptions> appOptionsAccessor)
		{
			if (urlHelper == null)
			{
				throw new ArgumentNullException(nameof(urlHelper));
			}

			if (appOptionsAccessor == null)
			{
				throw new ArgumentNullException(nameof(appOptionsAccessor));
			}
			_urlHelper = urlHelper;
			_appOptions = appOptionsAccessor.Value;
		}

		public IViewComponentResult Invoke(string assetType)
		{
			string viewName;
			IList<string> filePaths = new List<string>();

			var versionQueryString = _appOptions.Versioning.EnableStaticContentVersioning ? $"?v={_appOptions.Versioning.BuildNumber}" : string.Empty;
			if (assetType.Equals("scripts", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Scripts";
				if (_appOptions.EnableBundlingAndMinification)
				{
				    if (_minifiedJsName == null)
				    {
                        var assetInfo = FrontEndAssetManager.GetScripts();
                        _minifiedJsName = assetInfo.MinifiedFileName;
				    }
					filePaths.Add(_urlHelper.Content(
						$"~/{_minifiedJsName}.min.js{versionQueryString}"));
				}
				else
				{
                    var assetInfo = FrontEndAssetManager.GetScripts();
                    foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(_urlHelper.Content($"~/{assetFileInfo}{versionQueryString}"));
					}
				}
			}
			else if (assetType.Equals("styles", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Styles";
				if (_appOptions.EnableBundlingAndMinification)
				{
				    if (_minifiedCssName == null)
				    {
                        var assetInfo = FrontEndAssetManager.GetStyles();
				        _minifiedCssName = assetInfo.MinifiedFileName;
				    }
					filePaths.Add(_urlHelper.Content(
						$"~/{_minifiedCssName}.min.css{versionQueryString}"));
				}
				else
				{
                    var assetInfo = FrontEndAssetManager.GetStyles();
                    foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(_urlHelper.Content($"~/{assetFileInfo}{versionQueryString}"));
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