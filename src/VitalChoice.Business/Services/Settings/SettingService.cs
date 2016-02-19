using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
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

        public Task<List<AppSettingItem>> GetAppSettingItemsAsync(ICollection<string> names)
        {
            return appSettingRepository.Query(p => names.Contains(p.Name)).SelectAsync(false);
        }

        public Task<AppSettingItem> GetAppSettingAsync(string name)
        {
            return appSettingRepository.Query(p => p.Name == name).SelectFirstOrDefaultAsync(false);
        }

        public async Task<List<AppSettingItem>> UpdateAppSettingItemsAsync(ICollection<AppSettingItem> items)
        {
            var names = items.Select(p => p.Name).ToArray();
            var dbItems = await appSettingRepository.Query(p => names.Contains(p.Name)).SelectAsync(true);
            foreach (var dbItem in dbItems)
            {
                foreach (var item in items)
                {
                    if (dbItem.Name == item.Name)
                    {
                        dbItem.Value = item.Value;
                    }
                }
            }
            await appSettingRepository.UpdateRangeAsync(dbItems);

            return dbItems;
        }

        public async Task<AppSettings> GetAppSettingsAsync()
        {
            var appSettings = await appSettingRepository.Query().SelectAsync(false);

            AppSettings toReturn = CreateAppSettings(appSettings);

            return toReturn;
        }

        private AppSettings CreateAppSettings(ICollection<AppSettingItem> items)
        {
            AppSettings toReturn = new AppSettings();
            foreach (var property in typeof(AppSettings).GetTypeInfo().GetProperties())
            {
                AppSettingItem setting = items.FirstOrDefault(p => p.Name == property.Name);
                if (setting != null)
                {
                    try
                    {
                        if (property.PropertyType == typeof(bool))
                        {
                            bool value;
                            if (bool.TryParse(setting.Value, out value))
                            {
                                property.SetValue(toReturn, value, null);
                            }
                        }
                        if (property.PropertyType == typeof(int?) || property.PropertyType == typeof(int))
                        {
                            int value;
                            if (int.TryParse(setting.Value, out value))
                            {
                                property.SetValue(toReturn, value, null);
                            }
                        }
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(toReturn, setting.Value, null);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogCritical(0, e.Message, e);
                        throw;
                    }
                }
            }
            return toReturn;
        }
    }
}