using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace VitalChoice.Ecommerce.Utils
{
    public static class EnumHelper
    {
	    public static Dictionary<T, string> GetItemsWithDescription<T>(Type enumType)
	    {
            var values = Enum.GetValues(enumType);

            return values.Cast<object>().ToDictionary(value => (T) value, value =>
		    {
#if NET451
			    return ((Enum) value).GetAttributeOfType<DescriptionAttribute>().Description;
#else
				return Enum.GetName(enumType, value);
#endif
			});
	    }

        public static Dictionary<T, string> GetItems<T>(Type enumType)
        {
            var values = Enum.GetValues(enumType);

            return values.Cast<object>().ToDictionary(value => (T)value, value => value.ToString() );
        }

        public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
		{
#if NET451
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
			return (attributes.Length > 0) ? (T)attributes[0] : null;
#else
			return null;
#endif
		}
	}
}