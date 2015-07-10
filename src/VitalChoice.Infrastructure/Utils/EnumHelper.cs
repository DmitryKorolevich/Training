using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.Data.Entity.Query;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Utils
{
    public static class EnumHelper
    {
        public static Dictionary<T, string> GetItemsWithDescription<T>(Type enumType)
        {
            var values = Enum.GetValues(enumType);

            return values.Cast<object>()
                .ToDictionary(value => (T) value, value => ((Enum) value).GetEnumDescription());
        }

        public static Dictionary<T, string> GetItems<T>(Type enumType)
        {
            var values = Enum.GetValues(enumType);

            return values.Cast<object>().ToDictionary(value => (T)value, value => value.ToString() );
        }


        public static string GetEnumDescription(this Enum enumVal)
		{
#if !DNXCORE50
            var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes<DescriptionAttribute>(false).ToArray();
			return (attributes.Length > 0) ? attributes[0].Description : enumVal.ToString();
#else
            return enumVal.ToString();
#endif
        }
    }
}