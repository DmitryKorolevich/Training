using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using System.Reflection;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VC.Admin.Models.Setting
{
    public class GlobalSettingsManageModel : BaseModel
    {
        public int? GlobalPerishableThreshold { get; set; }

        public bool CreditCardAuthorizations { get; set; }

        public GlobalSettingsManageModel()
        {
        }

        public GlobalSettingsManageModel(ICollection<AppSettingItem> items)
        {
#if !DOTNET5_4
            foreach (var property in typeof(GlobalSettingsManageModel).GetProperties())
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
                                property.SetValue(this, toSet, null);
                            }
                        }
                        if (property.PropertyType == typeof(int?))
                        {
                            int toSet = 0;
                            if (int.TryParse(setting.Value, out toSet))
                            {
                                property.SetValue(this, toSet, null);
                            }
                        }
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(this, setting.Value, null);
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerService.GetDefault().LogCritical(0, e.Message, e);
                        throw;
                    }
                }
            }
#endif
        }

        public List<AppSettingItem> Convert()
        {
            List<AppSettingItem> toReturn = new List<AppSettingItem>();
#if DNX451
            foreach (var property in typeof(GlobalSettingsManageModel).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            {
                string value = null;
                value = property.GetValue(this)?.ToString();
                toReturn.Add(new AppSettingItem()
                {
                    Name = property.Name,
                    Value=value,
                });
            }
#endif
            return toReturn;
        }
    }
}