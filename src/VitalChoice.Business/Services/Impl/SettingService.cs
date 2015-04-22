using Microsoft.Framework.ConfigurationModel;
using VitalChoice.Business.Services.Contracts;

namespace VitalChoice.Business.Services.Impl
{
	public class SettingService : ISettingService
    {
        protected IConfiguration Configuration { get; private set; }

        public SettingService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GetProjectConstant(string key)
        {
            return Configuration.Get("Constants:" + key);
        }
    }
}
