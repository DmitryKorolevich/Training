using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;

namespace VitalChoice.Business.Services.Settings
{
    public class SettingService : ISettingService
    {
        private readonly IRepositoryAsync<AppSettingItem> _appSettingRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly ILogger _logger;

        public SettingService(IRepositoryAsync<AppSettingItem> appSettingRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository,
            ILoggerProviderExtended loggerProvider)
        {
            _appSettingRepository = appSettingRepository;
            _lookupRepository = lookupRepository;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public Task<List<AppSettingItem>> GetAppSettingItemsAsync(ICollection<string> names)
        {
            return _appSettingRepository.Query(p => names.Contains(p.Name)).SelectAsync(false);
        }

        public Task<AppSettingItem> GetAppSettingAsync(string name)
        {
            return _appSettingRepository.Query(p => p.Name == name).SelectFirstOrDefaultAsync(false);
        }

        public async Task<List<AppSettingItem>> UpdateAppSettingItemsAsync(ICollection<AppSettingItem> items)
        {
            var names = items.Select(p => p.Name).ToArray();
            var dbItems = await _appSettingRepository.Query(p => names.Contains(p.Name)).SelectAsync(true);
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
            await _appSettingRepository.UpdateRangeAsync(dbItems);

            return dbItems;
        }

        public async Task<AppSettings> GetAppSettingsAsync()
        {
            var appSettings = await _appSettingRepository.Query().SelectAsync(false);

            AppSettings toReturn = CreateAppSettings(appSettings);

            return toReturn;
        }

        private AppSettings CreateAppSettings(ICollection<AppSettingItem> items)
        {
            AppSettings toReturn = new AppSettings();
            foreach (var property in typeof(AppSettings).GetRuntimeProperties())
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
                        _logger.LogCritical(0, e.Message, e);
                        throw;
                    }
                }
            }
            return toReturn;
        }

        public async Task<IList<Lookup>> GetLookupsAsync(ICollection<string> names)
        {
            if (names == null || !names.Any())
            {
                return new List<Lookup>();
            }
            return await _lookupRepository.Query(p => names.Contains(p.Name)).Include(p => p.LookupVariants).SelectAsync(false);
        }

        public async Task<Lookup> GetLookupAsync(int id)
        {
            var toReturn = await _lookupRepository.Query(p => p.Id==id).Include(p=>p.LookupVariants).SelectFirstOrDefaultAsync(false);
            toReturn.LookupVariants = toReturn.LookupVariants.OrderBy(p => p.Order).ToList();
            return toReturn;
        }

        public async Task<bool> UpdateLookupVariantsAsync(int id, ICollection<LookupVariant> variants)
        {
            int order = 1;
            foreach (var lookupVariant in variants)
            {
                lookupVariant.Order = order;
                order++;
                if (lookupVariant.Id == 0)
                {
                    lookupVariant.Id = variants.Max(p => p.Id)+1;
                }
            }

            var dbLookup = (await _lookupRepository.Query(p => p.Id == id).Include(p => p.LookupVariants).SelectAsync(true)).FirstOrDefault();
            if (dbLookup != null)
            {
                dbLookup.LookupVariants.MergeKeyed(variants, p=>p.Id, (a,b) =>
                {
                    a.ValueVariant = b.ValueVariant;
                    a.Order = b.Order;
                });

                try
                {
                    await _lookupRepository.UpdateAsync(dbLookup);
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException != null && e.InnerException is SqlException &&
                        (e.InnerException as SqlException).Number == SqlConstants.ERROR_CODE_FOREIGN_KEY_CONFLICT)
                    {
                        throw new AppValidationException(string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.DenyDeleteInUseItems],"Variants"));
                    }
                }
            }

            return true;
        }
    }
}