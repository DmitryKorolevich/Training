using Microsoft.Framework.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Business.Services.Contracts.Settings;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Business.Services.Impl.Settings
{
    public class SettingService : ISettingService
    {
        private readonly IRepositoryAsync<AppSettingItem> appSettingRepository;
        private readonly ILogger logger;

        public SettingService(IRepositoryAsync<AppSettingItem> appSettingRepository)
        {
            this.appSettingRepository = appSettingRepository;
            logger = LoggerService.GetDefault();
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
            var appSettings = (await appSettingRepository.Query().SelectAsync(false)).ToList();

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