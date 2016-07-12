﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Interfaces.Services.Settings;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VitalChoice.Business.Services.Settings
{
    public class SettingService : ISettingService
    {
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly IEcommerceRepositoryAsync<SettingOptionType> _settingOptionTypeRepository;
        private readonly SettingMapper _settingMapper;

        public SettingService(
            IEcommerceRepositoryAsync<Lookup> lookupRepository,
            IEcommerceRepositoryAsync<SettingOptionType> settingOptionTypeRepository,
            SettingMapper settingMapper)
        {
            _lookupRepository = lookupRepository;
            _settingOptionTypeRepository = settingOptionTypeRepository;
            _settingMapper = settingMapper;
        }

        public Task<AppSettings> GetSettingsInstanceAsync() => _settingMapper.CreatePrototypeForAsync<AppSettings>();

        public AppSettings GetSettingsInstance() => _settingMapper.CreatePrototypeFor<AppSettings>();

        public SettingDynamic GetSettings() => _settingMapper.CreatePrototype();

        public Task<SettingDynamic> GetSettingsAsync() => _settingMapper.CreatePrototypeAsync();

        public async Task<bool> UpdateSettingsAsync(SettingDynamic settings)
        {
            var entity = await _settingMapper.ToEntityAsync(settings);
            var optionTypes = await _settingOptionTypeRepository.Query().SelectAsync(true);
            var forUpdate = new List<SettingOptionType>();
            foreach (var settingOptionValue in entity.OptionValues)
            {
                var optionType = optionTypes.FirstOrDefault(p => p.Id == settingOptionValue.IdOptionType);
                if (optionType != null && optionType.DefaultValue != settingOptionValue.Value)
                {
                    optionType.DefaultValue = settingOptionValue.Value;
                    forUpdate.Add(optionType);
                }
            }
            await _settingOptionTypeRepository.UpdateRangeAsync(forUpdate);
            return true;
        }

        public async Task<IList<Lookup>> GetLookupsAsync(ICollection<string> names)
        {
            if (names == null || names.Count == 0)
            {
                return new List<Lookup>();
            }
            return await _lookupRepository.Query(p => names.Contains(p.Name)).Include(p => p.LookupVariants).SelectAsync(false);
        }

        public async Task<Lookup> GetLookupAsync(int id)
        {
            var toReturn = await _lookupRepository.Query(p => p.Id == id).Include(p => p.LookupVariants).SelectFirstOrDefaultAsync(false);
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
                    lookupVariant.Id = variants.Max(p => p.Id) + 1;
                }
            }

            var dbLookup =
                (await _lookupRepository.Query(p => p.Id == id).Include(p => p.LookupVariants).SelectFirstOrDefaultAsync(true));
            if (dbLookup != null)
            {
                dbLookup.LookupVariants.MergeKeyed(variants, p => p.Id, (a, b) =>
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
                        throw new AppValidationException(
                            string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.DenyDeleteInUseItems], "Variants"));
                    }
                }
            }

            return true;
        }
    }
}