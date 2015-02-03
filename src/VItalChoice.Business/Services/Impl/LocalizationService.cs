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

namespace VitalChoice.Business.Services.Impl
{
	public class LocalizationService : ILocalizationService
	{
        private Dictionary<int, Dictionary<int, List<LocalizationItemData>>> _localizationData = null;
        private object _localizationDataCreateLock = new object();

        protected Dictionary<int, Dictionary<int, List<LocalizationItemData>>> LocalizationData
        {
            get
            {
                if(_localizationData==null)
                {
                    lock(_localizationDataCreateLock)
                    {
                        if (_localizationData == null)
                        {
                            var localizationData = Repository.Query().Select().ToList();
                            _localizationData = new Dictionary<int, Dictionary<int, List<LocalizationItemData>>>();
                            foreach (var localizationDataItem in localizationData)
                            {
                                Dictionary<int, List<LocalizationItemData>> group = null;
                                if (_localizationData.ContainsKey(localizationDataItem.GroupId))
                                {
                                    group = _localizationData[localizationDataItem.GroupId];
                                }
                                else
                                {
                                    group = new Dictionary<int, List<LocalizationItemData>>();
                                    _localizationData.Add(localizationDataItem.GroupId,group);
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
                    }
                }

                return _localizationData;
            }
        }

        protected IRepositoryAsync<LocalizationItemData> Repository { get; private set; }

        protected ISettingService SettingService { get; private set; }

        public LocalizationService(IRepositoryAsync<LocalizationItemData> repository, ISettingService settingService)
        {
            Repository = repository;
            SettingService = settingService;
        }

        public string GetString<TEnum>(TEnum enumValue) where TEnum : struct, IComparable, IFormattable
        {
            return GetDirectString(enumValue, GetCultureCode());
        }

        public string GetString<TEnum>(TEnum enumValue, params object[] args) where TEnum : struct, IComparable, IFormattable
        {
            return GetDirectString(enumValue, GetCultureCode(), args);
        }

        public string GetDirectString<TEnum>(TEnum enumValue, string cultureId, params object[] args) where TEnum : struct, IComparable, IFormattable
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
        private string GetStringFromLocalizationDataItem(int groupId, int itemId, string cultureId, params object[] args)
        {
            string toReturn = null;
            var data = LocalizationData;
            if (data != null)
            {
                Dictionary<int, List<LocalizationItemData>> group = null;
                if(data.TryGetValue(groupId, out group))
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
                            if(defaultCulureId!=null)
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

        private string GetCultureCode()
        {
#if ASPNET50
            return Thread.CurrentThread.CurrentCulture.Name ?? SettingService.GetProjectConstant("DefaultCultureId");
#else
            return SettingService.GetProjectConstant("DefaultCultureId");
#endif
        }
    }
}
