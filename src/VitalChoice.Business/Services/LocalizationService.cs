using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Ecommerce.Utils;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities.Localization;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Business.Services
{
    public class LocalizationService : ILocalizationService
    {
        public static LocalizationService Current { get; private set; }

        public LocalizationService(IOptions<AppOptions> options, DbContextOptions<VitalChoiceContext> contextOptions, string defaultCultureId)
        {
            _defaultCultureId = defaultCultureId;
            using (VitalChoiceContext context = new VitalChoiceContext(options, contextOptions))
            {
                CreateLocalizationData(new RepositoryAsync<LocalizationItemData>(context));
            }
            Current = this;
        }

        private const string DOMAIN_ASSEMBLY_NAME = "VitalChoice.Infrastructure.Domain";
        private const string LOCALIZATION_GROUPS_NAMESPACE = "VitalChoice.Infrastructure.Domain.Entities.Localization.Groups";
        private const string NO_LABEL_VALUES = "Not defined";

        private Dictionary<int, Dictionary<int, List<LocalizationItemData>>> _localizationData;

        private void CreateLocalizationData(IRepositoryAsync<LocalizationItemData> repository)
        {
            _localizationData = new Dictionary<int, Dictionary<int, List<LocalizationItemData>>>();
            var dbLocalizationData = repository.Query().Select().ToList();
            foreach (var localizationDataItem in dbLocalizationData)
            {
                Dictionary<int, List<LocalizationItemData>> group = null;
                if (_localizationData.ContainsKey(localizationDataItem.GroupId))
                {
                    group = _localizationData[localizationDataItem.GroupId];
                }
                else
                {
                    group = new Dictionary<int, List<LocalizationItemData>>();
                    _localizationData.Add(localizationDataItem.GroupId, group);
                }
                List<LocalizationItemData> items = null;
                if (group.ContainsKey(localizationDataItem.ItemId))
                {
                    items = group[localizationDataItem.ItemId];
                }
                else
                {
                    items = new List<LocalizationItemData>();
                    group.Add(localizationDataItem.ItemId, items);
                }
                items.Add(localizationDataItem);
            }
        }


        private readonly string _defaultCultureId;

        public IList<LookupItem<string>> GetStrings()
        {
            return GetStrings(GetCultureCode());
        }

        public IList<LookupItem<string>> GetStrings(string cultureId)
        {
            List<LookupItem<string>> toReturn = new List<LookupItem<string>>();

#if !NETSTANDARD1_5
            if (_localizationData != null)
            {
                var assembly =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(p => p.GetName().Name == DOMAIN_ASSEMBLY_NAME);
                if (assembly != null)
                {
                    var types =
                        assembly.GetTypes()
                            .Where(p => p.IsEnum && p.Namespace == LocalizationService.LOCALIZATION_GROUPS_NAMESPACE)
                            .ToList();
                    foreach (var type in types)
                    {
                        var typeInfo = type.GetTypeInfo();
                        var localizationGroupAttribute =
                            typeInfo.GetCustomAttributes(typeof (LocalizationGroupAttribute), true).SingleOrDefault() as
                                LocalizationGroupAttribute;
                        if (localizationGroupAttribute == null)
                        {
                            throw new ArgumentException(
                                $"LocalizationGroupAttribute isn't set on the given enum property {typeInfo.FullName}.");
                        }

                        Dictionary<int, List<LocalizationItemData>> group = null;
                        if (_localizationData.TryGetValue(localizationGroupAttribute.GroupId, out @group))
                        {
                            var items = EnumHelper.GetItems<byte>(type);
                            foreach (var item in items)
                            {
                                var label = GetItemValue(@group, item.Key, cultureId);
                                toReturn.Add(new LookupItem<string>
                                {
                                    Key = $"{type.Name}.{item.Value}",
                                    Text = label,
                                });
                            }
                        }
                    }
                }
            }
#endif
            return toReturn;
        }

        public string GetString(object enumValue)
        {
            return GetDirectString(enumValue, GetCultureCode());
        }

        public string GetString(object enumValue, params object[] args)
        {
            return GetDirectString(enumValue, GetCultureCode(), args);
        }

        public string GetDirectString(object enumValue, string cultureId, params object[] args)
        {
            var enumType = enumValue.GetType().GetTypeInfo();

            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"Not enum type {enumType.FullName}.");
            }
            var localizationGroupAttribute =
                enumType.GetCustomAttributes(typeof (LocalizationGroupAttribute), true).SingleOrDefault() as
                    LocalizationGroupAttribute;
            if (localizationGroupAttribute == null)
            {
                throw new ArgumentException(
                    $"LocalizationGroupAttribute isn't set on the given enum property {enumType.FullName}.");
            }

            return GetStringFromLocalizationDataItem(localizationGroupAttribute.GroupId, Convert.ToInt32(enumValue),
                cultureId, args);
        }

        private string GetStringFromLocalizationDataItem(int groupId, int itemId, string cultureId, params object[] args)
        {
            string toReturn = null;
            if (_localizationData != null)
            {
                Dictionary<int, List<LocalizationItemData>> group = null;
                if (_localizationData.TryGetValue(groupId, out group))
                {
                    toReturn = GetItemValue(group, itemId, cultureId, args);
                }
            }

            return toReturn;
        }

        private string GetItemValue(Dictionary<int, List<LocalizationItemData>> group, int itemId, string cultureId,
            params object[] args)
        {
            string toReturn = null;
            if (group != null)
            {
                List<LocalizationItemData> items = null;
                if (group.TryGetValue(itemId, out items))
                {
                    var item = items.FirstOrDefault(p => p.CultureId == cultureId);
                    //Check language part
                    if (item == null)
                    {
                        if (cultureId != null && cultureId.Length > 2)
                        {
                            item = items.FirstOrDefault(p => p.CultureId == cultureId.Substring(0, 2));
                        }
                    }
                    //Check default label
                    if (item == null)
                    {
                        var defaultCulureId = _defaultCultureId;
                        if (defaultCulureId != null)
                        {
                            item = items.FirstOrDefault(p => p.CultureId == defaultCulureId);
                        }
                        if (item == null)
                        {
                            if (defaultCulureId != null && defaultCulureId.Length > 2)
                            {
                                item = items.FirstOrDefault(p => p.CultureId == defaultCulureId.Substring(0, 2));
                            }
                        }
                    }
                    if (item != null)
                    {
                        if (args != null && args.Length > 0)
                        {
                            toReturn = String.Format(item.Value, args);
                        }
                        else
                        {
                            toReturn = item.Value;
                        }
                    }
                    else
                    {
                        toReturn = LocalizationService.NO_LABEL_VALUES;
                    }
                }
            }
            return toReturn;
        }

        private string GetCultureCode()
        {
            return _defaultCultureId;
        }
    }
}