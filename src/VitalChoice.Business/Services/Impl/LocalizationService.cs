using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Infrastructure.Utils;

namespace VitalChoice.Business.Services.Impl
{
    public static class LocalizationService
    {
        private const string DOMAIN_ASSEMBLY_NAME = "VitalChoice.Domain";
        private const string LOCALIZATION_GROUPS_NAMESPACE = "VitalChoice.Domain.Entities.Localization.Groups";
        private const string NO_LABEL_VALUES = "Not defined";

        private static Dictionary<int, Dictionary<int, List<LocalizationItemData>>> localizationData;

        private static void CreateLocalizationData()
        {
            localizationData = new Dictionary<int, Dictionary<int, List<LocalizationItemData>>>();
            var dbLocalizationData = repository.Query().Select().ToList();
            foreach (var localizationDataItem in dbLocalizationData)
            {
                Dictionary<int, List<LocalizationItemData>> group = null;
                if (localizationData.ContainsKey(localizationDataItem.GroupId))
                {
                    group = localizationData[localizationDataItem.GroupId];
                }
                else
                {
                    group = new Dictionary<int, List<LocalizationItemData>>();
                    localizationData.Add(localizationDataItem.GroupId, group);
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

        private static IRepositoryAsync<LocalizationItemData> repository;

        private static string defaultCultureId;

        public static void Init(IRepositoryAsync<LocalizationItemData> repository, string defaultCultureId)
        {
            LocalizationService.repository = repository;
            LocalizationService.defaultCultureId = defaultCultureId;
            CreateLocalizationData();
        }

        public static IList<LookupItem<string>> GetStrings()
        {
            return GetStrings(GetCultureCode());
        }

        public static IList<LookupItem<string>> GetStrings(string cultureId)
        {
            List<LookupItem<string>> toReturn = new List<LookupItem<string>>();
#if DNX451
            string label = null;
            if (localizationData != null)
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(p => p.GetName().Name == LocalizationService.DOMAIN_ASSEMBLY_NAME).FirstOrDefault();
                var types = assembly.GetTypes().Where(p => p.IsEnum && p.Namespace == LocalizationService.LOCALIZATION_GROUPS_NAMESPACE).ToList();
                foreach (var type in types)
                {
                    var typeInfo = type.GetTypeInfo();
                    var localizationGroupAttribute = typeInfo.GetCustomAttributes(typeof(LocalizationGroupAttribute), true).SingleOrDefault() as
                        LocalizationGroupAttribute;
                    if (localizationGroupAttribute == null)
                    {
                        throw new ArgumentException(string.Format("LocalizationGroupAttribute isn't set on the given enum property {0}.", typeInfo.FullName));
                    }

                    Dictionary<int, List<LocalizationItemData>> group = null;
                    if (localizationData.TryGetValue(localizationGroupAttribute.GroupId, out group))
                    {
                        var items = EnumHelper.GetItems<byte>(type);
                        foreach (var item in items)
                        {
                            label = GetItemValue(group, item.Key, cultureId);
                            toReturn.Add(new LookupItem<string>()
                            {
                                Key= String.Format("{0}.{1}", type.Name,item.Value),
                                Text = label,
                            });
                        }
                    }
                }
            }
#endif
            return toReturn;
        }

        public static string GetString(object enumValue)
        {
            return GetDirectString(enumValue, GetCultureCode());
        }

        public static string GetString(object enumValue, params object[] args)
        {
            return GetDirectString(enumValue, GetCultureCode(), args);
        }

        public static string GetDirectString(object enumValue, string cultureId, params object[] args)
        {
            var enumType = enumValue.GetType().GetTypeInfo();

            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("Not enum type {0}.", enumType.FullName));
            }
            var localizationGroupAttribute = enumType.GetCustomAttributes(typeof(LocalizationGroupAttribute), true).SingleOrDefault() as
                LocalizationGroupAttribute;
            if (localizationGroupAttribute == null)
            {
                throw new ArgumentException(string.Format("LocalizationGroupAttribute isn't set on the given enum property {0}.", enumType.FullName));
            }

            return GetStringFromLocalizationDataItem(localizationGroupAttribute.GroupId, Convert.ToInt32(enumValue), cultureId, args);
        }
        private static string GetStringFromLocalizationDataItem(int groupId, int itemId, string cultureId, params object[] args)
        {
            string toReturn = null;
            if (localizationData != null)
            {
                Dictionary<int, List<LocalizationItemData>> group = null;
                if (localizationData.TryGetValue(groupId, out group))
                {
                    toReturn = GetItemValue(group, itemId, cultureId, args);
                }
            }

            return toReturn;
        }

        private static string GetItemValue(Dictionary<int, List<LocalizationItemData>> group, int itemId, string cultureId, params object[] args)
        {
            string toReturn = null;
            if (group != null)
            {
                List<LocalizationItemData> items = null;
                if (group.TryGetValue(itemId, out items))
                {
                    var item = items.Where(p => p.CultureId == cultureId).FirstOrDefault();
                    //Check language part
                    if (item == null)
                    {
                        if (cultureId != null && cultureId.Length > 2)
                        {
                            item = items.Where(p => p.CultureId == cultureId.Substring(0, 2)).FirstOrDefault();
                        }
                    }
                    //Check default label
                    if (item == null)
                    {
                        var defaultCulureId = LocalizationService.defaultCultureId;
                        if (defaultCulureId != null)
                        {
                            item = items.Where(p => p.CultureId == defaultCulureId).FirstOrDefault();
                        }
                        if (item == null)
                        {
                            if (defaultCulureId != null && defaultCulureId.Length > 2)
                            {
                                item = items.Where(p => p.CultureId == defaultCulureId.Substring(0, 2)).FirstOrDefault();
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

        private static string GetCultureCode()
        {
#if DNX451
            //return Thread.CurrentThread.CurrentCulture.Name ?? LocalizationService.defaultCultureId;
            return LocalizationService.defaultCultureId;
#else
            return LocalizationService.defaultCultureId;
#endif
        }
    }
}
