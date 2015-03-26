using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Infrastructure.Models;

namespace VitalChoice.Public.Components
{
	[ViewComponent(Name = "Assets")]
	public class AssetsViewComponent : ViewComponent
	{
		private readonly IOptions<AppOptions> appOptionsAccessor;
		private readonly FrontEndAssetManager assetManager;
		private readonly IUrlHelper urlHelper;

		public AssetsViewComponent(FrontEndAssetManager assetManager, IUrlHelper urlHelper, IOptions<AppOptions> appOptionsAccessor)
		{
			if (assetManager == null)
			{
				throw new ArgumentNullException("assetManager");
			}

			if (urlHelper == null)
			{
				throw new ArgumentNullException("urlHelper");
			}

			if (appOptionsAccessor == null)
			{
				throw new ArgumentNullException("appOptionsAccessor");
			}

			this.assetManager = assetManager;
			this.urlHelper = urlHelper;
			this.appOptionsAccessor = appOptionsAccessor;
		}

		public IViewComponentResult Invoke(string assetType)
		{
			string viewName;
			IList<string> filePaths = new List<string>();

			if (assetType.Equals("scripts", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Scripts";
				AssetInfo assetInfo = assetManager.GetScripts();
				if (appOptionsAccessor.Options.EnableBundlingAndMinification)
				{
					filePaths.Add(urlHelper.Content(string.Format("~/{0}.min-{1}.js", assetInfo.MinifiedFileName, appOptionsAccessor.Options.RandomPathPart)));
				}
				else
				{
					foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(urlHelper.Content(string.Format("~/{0}", assetFileInfo)));
					}
				}
			}
			else if (assetType.Equals("styles", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Styles";
				AssetInfo assetInfo = assetManager.GetStyles();
				if (appOptionsAccessor.Options.EnableBundlingAndMinification)
				{
					filePaths.Add(urlHelper.Content(string.Format("~{0}.min-{1}.css", assetInfo.MinifiedFileName, appOptionsAccessor.Options.RandomPathPart)));
				}
				else
				{
					foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(urlHelper.Content(string.Format("~/{0}", assetFileInfo)));
					}
				}
			}
			else
			{
				throw new InvalidOperationException(string.Format("Asset type '{0}' is not recognized."));
			}

			return View(viewName, filePaths);
		}
	}
}