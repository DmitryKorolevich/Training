using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VitalChoice.Core.Infrastructure.Models;

namespace VitalChoice.Core.Infrastructure
{
	public class FrontEndAssetManager : IFrontEndAssetManager
	{
		private const string ScriptsFilePath = "AppConfig/scripts/files.json";
		private const string StylesFilePath = "AppConfig/styles/files.json";
        private const string StylesOrderInvoiceFilePath = "AppConfig/styles-order-invoice/files.json";

        private readonly string _scriptsAppRelativePath;
		private readonly string _stylesAppRelativePath;
        private readonly string _stylesOrderInvoiceAppRelativePath;
        private readonly JsonSerializerSettings _serializerSettings;

		public FrontEndAssetManager(IHostingEnvironment env)
		{
			_scriptsAppRelativePath = Path.Combine(env.ContentRootPath, ScriptsFilePath);
			_stylesAppRelativePath = Path.Combine(env.ContentRootPath, StylesFilePath);
            _stylesOrderInvoiceAppRelativePath = Path.Combine(env.ContentRootPath, StylesOrderInvoiceFilePath);
            _serializerSettings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}

		public AssetInfo GetScripts()
		{
			return GetAssetInfo(_scriptsAppRelativePath);
		}

		public AssetInfo GetStyles()
		{
			return GetAssetInfo(_stylesAppRelativePath);
		}

        public AssetInfo GetOrderInvoiceStyles()
        {
            return GetAssetInfo(_stylesOrderInvoiceAppRelativePath);
        }

        private AssetInfo GetAssetInfo(string filePath)
		{
		    using (Stream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
		    {
		        using (StreamReader reader = new StreamReader(inputStream))
		        {
		            string jsonValue = reader.ReadToEnd();
		            return JsonConvert.DeserializeObject<AssetInfo>(jsonValue, _serializerSettings);
		        }
		    }
		}
	}
}