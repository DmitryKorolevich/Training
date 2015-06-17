using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Services;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;

namespace VitalChoice.Business.Services.Settings
{
    public class SettingService : ISettingService
    {
        private readonly IRepositoryAsync<AppSettingItem> appSettingRepository;
        private readonly ILogger logger;

        public SettingService(IRepositoryAsync<AppSettingItem> appSettingRepository, ILoggerProviderExtended loggerProvider)
        {
            this.appSettingRepository = appSettingRepository;
            logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<AppSettingItem> GetAppSettingAsync(int id)
        {
            var appSetting = (await appSettingRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();

            return appSetting;
        }

        public async Task<AppSettingItem> UpdateAppSettingAsync(int id, string value)
        {
            AppSettingItem toReturn = (await appSettingRepository.Query(p => p.Id == id).SelectAsync()).FirstOrDefault(); ;
            if(toReturn!=null)
            {
                toReturn.Value = value;
                await appSettingRepository.UpdateAsync(toReturn);
            }

            return toReturn;
        }

        public async Task<AppSettings> GetAppSettingsAsync()
        {
            var appSettings = await appSettingRepository.Query().SelectAsync(false);

            AppSettings toReturn = CreateAppSettings(appSettings);

            return toReturn;
        }

        private AppSettings CreateAppSettings(IEnumerable<AppSettingItem> items)
        {
            AppSettings toReturn = new AppSettings();
#if DNX451
            foreach (var property in typeof(AppSettings).GetProperties())
            {
                AppSettingItem setting = items.FirstOrDefault(p => p.Name == property.Name);
                if (setting != null)
                {
                    try
                    {
                        if (property.PropertyType == typeof(bool))
                        {
                            bool toSet = false;
                            if (bool.TryParse(setting.Value, out toSet))
                            {
                                property.SetValue(toReturn, toSet, null);
                            }
                        }
                        if (property.PropertyType == typeof(int?))
                        {
                            int toSet = 0;
                            if (int.TryParse(setting.Value, out toSet))
                            {
                                property.SetValue(toReturn, toSet, null);
                            }
                        }
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(toReturn, setting.Value, null);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
#endif
            return toReturn;
        }
    }
}