﻿using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VitalChoice.Core.Infrastructure.Models;

namespace VitalChoice.Core.Infrastructure
{
	public static class FrontEndAssetManager
	{
		private const string ScriptsFilePath = "AppConfig/scripts/files.json";
		private const string StylesFilePath = "AppConfig/styles/files.json";
        private const string StylesOrderInvoiceFilePath = "AppConfig/styles-order-invoice/files.json";

        private static readonly string scriptsAppRelativePath;
		private static readonly string stylesAppRelativePath;
        private static readonly string stylesOrderInvoiceAppRelativePath;
        private static readonly JsonSerializerSettings serializerSettings;

		static FrontEndAssetManager()
		{
			scriptsAppRelativePath = PathResolver.ResolveAppRelativePath(ScriptsFilePath);
			stylesAppRelativePath = PathResolver.ResolveAppRelativePath(StylesFilePath);
            stylesOrderInvoiceAppRelativePath = PathResolver.ResolveAppRelativePath(StylesOrderInvoiceFilePath);
            serializerSettings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}

		public static AssetInfo GetScripts()
		{
			return GetAssetInfo(scriptsAppRelativePath);
		}

		public static AssetInfo GetStyles()
		{
			return GetAssetInfo(stylesAppRelativePath);
		}

        public static AssetInfo GetOrderInvoiceStyles()
        {
            return GetAssetInfo(stylesOrderInvoiceAppRelativePath);
        }

        private static AssetInfo GetAssetInfo(string filePath)
		{
		    using (Stream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
		    {
		        using (StreamReader reader = new StreamReader(inputStream))
		        {
		            string jsonValue = reader.ReadToEnd();
		            return JsonConvert.DeserializeObject<AssetInfo>(jsonValue, serializerSettings);
		        }
		    }
		}
	}
}