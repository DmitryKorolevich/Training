using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
	public class StylesService : IStylesService
	{
		private readonly IOptions<AppOptions> _options;
		private string _customStylesFullPath;

		public StylesService(IOptions<AppOptions> options)
		{
			_options = options;

			_customStylesFullPath = $"{_options.Options.CustomStylesPath}\\{_options.Options.CustomStylesName}";
		}

		public string GetStyles()
		{
			return File.Exists(_customStylesFullPath) ? File.ReadAllText(_customStylesFullPath) : string.Empty;
		}

		public async Task<string> UpdateStylesAsync(string css)
		{
			using (var stream = File.Open(_customStylesFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
			{
				using (var writer = new StreamWriter(stream, Encoding.UTF8))
				{
					await writer.WriteAsync(css);
					stream.SetLength(stream.Position);
				}
			}

			_options.Options.Versioning.CustomCssVersion = _options.Options.Versioning.CustomCssVersion + 1;

			return GetStyles();
		}
	}
}
