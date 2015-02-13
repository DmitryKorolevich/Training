using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Infrastructure.UnitOfWork;
using System.Threading;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Cache.Memory;
using VitalChoice.Domain.Entities.Localization.Groups;

namespace VitalChoice.Business.Services.Impl
{
	public static class LocalizationService
	{
        public static Dictionary<int, Dictionary<int, List<LocalizationItemData>>> LocalizationData;

        private static void CreateLocalizationData()
        {
            LocalizationData = new Dictionary<int, Dictionary<int, List<LocalizationItemData>>>();
            var localizationData = Repository.Query().Select().ToList();
            foreach (var localizationDataItem in localizationData)
            {
                Dictionary<int, List<LocalizationItemData>> group = null;
                if (LocalizationData.ContainsKey(localizationDataItem.GroupId))
                {
                    group = LocalizationData[localizationDataItem.GroupId];
                }
                else
                {
                    group = new Dictionary<int, List<LocalizationItemData>>();
                    LocalizationData.Add(localizationDataItem.GroupId, group);
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

        public static IRepositoryAsync<LocalizationItemData> Repository;

        public static ISettingService SettingService;

        public static void Init(IRepositoryAsync<LocalizationItemData> repository, ISettingService settingService)
        {
            Repository = repository;
            SettingService = settingService;
            CreateLocalizationData();
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
            if(localizationGroupAttribute==null)
            {
                throw new ArgumentException(string.Format("LocalizationGroupAttribute isn't set on the given enum property {0}.", enumType.FullName));
            }

            return GetStringFromLocalizationDataItem(localizationGroupAttribute.GroupId, Convert.ToInt32(enumValue), cultureId, args);
        }
        private static string GetStringFromLocalizationDataItem(int groupId, int itemId, string cultureId, params object[] args)
        {
            string toReturn = null;
            if (LocalizationService.LocalizationData != null)
            {
                Dictionary <int, List<LocalizationItemData>> group = null;
                if(LocalizationService.LocalizationData.TryGetValue(groupId, out group))
                {
                    List<LocalizationItemData> items = null;
                    if (group.TryGetValue(itemId, out items))
                    {
                        var item = items.Where(p => p.CultureId == cultureId).FirstOrDefault();
                        //Check language part
                        if (item == null)
                        {
                            if (cultureId!=null && cultureId.Length > 2)
                            {
                                item = items.Where(p => p.CultureId == cultureId.Substring(0,2)).FirstOrDefault();
                            }
                        }
                        //Check default label
                        if (item == null)
                        {
                            var defaultCulureId = SettingService.GetProjectConstant("DefaultCultureId");
                            if (defaultCulureId!=null)
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
                        if(item!=null)
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
                    }
                }                
            }
            
            return toReturn;
        }

        private static string GetCultureCode()
        {
#if ASPNET50
            return Thread.CurrentThread.CurrentCulture.Name ?? SettingService.GetProjectConstant("DefaultCultureId");
#else
            return SettingService.GetProjectConstant("DefaultCultureId");
#endif
        }
    }
}
