using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Domain.Entities.Options;

namespace VC.Admin.Components
{
	[ViewComponent(Name = "Assets")]
	public class AssetsViewComponent : ViewComponent
	{
		private readonly IOptions<AppOptions> _appOptionsAccessor;
		private readonly FrontEndAssetManager _assetManager;
		private readonly IUrlHelper _urlHelper;

		public AssetsViewComponent(FrontEndAssetManager assetManager, IUrlHelper urlHelper, IOptions<AppOptions> appOptionsAccessor)
		{
			if (assetManager == null)
			{
				throw new ArgumentNullException(nameof(assetManager));
			}

			if (urlHelper == null)
			{
				throw new ArgumentNullException(nameof(urlHelper));
			}

			if (appOptionsAccessor == null)
			{
				throw new ArgumentNullException(nameof(appOptionsAccessor));
			}

			_assetManager = assetManager;
			_urlHelper = urlHelper;
			_appOptionsAccessor = appOptionsAccessor;
		}

		public IViewComponentResult Invoke(string assetType)
		{
			string viewName;
			IList<string> filePaths = new List<string>();

			if (assetType.Equals("scripts", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Scripts";
				AssetInfo assetInfo = _assetManager.GetScripts();
				if (_appOptionsAccessor.Options.EnableBundlingAndMinification)
				{
					filePaths.Add(_urlHelper.Content(
					    $"~/{assetInfo.MinifiedFileName}.min-{_appOptionsAccessor.Options.RandomPathPart}.js"));
				}
				else
				{
					foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(_urlHelper.Content($"~/{assetFileInfo}"));
					}
				}
			}
			else if (assetType.Equals("styles", StringComparison.OrdinalIgnoreCase))
			{
				viewName = "Styles";
				AssetInfo assetInfo = _assetManager.GetStyles();
				if (_appOptionsAccessor.Options.EnableBundlingAndMinification)
				{
					filePaths.Add(_urlHelper.Content(
					    $"~{assetInfo.MinifiedFileName}.min-{_appOptionsAccessor.Options.RandomPathPart}.css"));
				}
				else
				{
					foreach (var assetFileInfo in assetInfo.Files)
					{
						filePaths.Add(_urlHelper.Content($"~/{assetFileInfo}"));
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