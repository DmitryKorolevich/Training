﻿using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Infrastructure.Domain.Options;

namespace VC.Public.Components
{
	[ViewComponent(Name = "Assets")]
	public class AssetsViewComponent : ViewComponent
	{
		private readonly AppOptions _appOptions;
		private readonly IUrlHelper _urlHelper;

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
				var assetInfo = FrontEndAssetManager.GetScripts();
				if (_appOptions.EnableBundlingAndMinification)
				{
					filePaths.Add(_urlHelper.Content(
						$"~/{assetInfo.MinifiedFileName}.min.js{versionQueryString}"));
				}
				else
				{
					foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(_urlHelper.Content($"~/{assetFileInfo}{versionQueryString}"));
					}
				}
			}
			else if (assetType.Equals("styles", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Styles";
				var assetInfo = FrontEndAssetManager.GetStyles();
				if (_appOptions.EnableBundlingAndMinification)
				{
					filePaths.Add(_urlHelper.Content(
						$"~/{assetInfo.MinifiedFileName}.min.css{versionQueryString}"));
				}
				else
				{
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