using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Data.Entity.Query;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Utils
{
    public static class EnumHelper
    {
	    public static Dictionary<int, string> GetItems(Type enumType)
	    {
		    var values = Enum.GetValues(enumType);

		    return values.Cast<object>().ToDictionary(value => (int) value, value =>
		    {
#if DNX451
			    return ((Enum) value).GetAttributeOfType<DescriptionAttribute>().Description;
#else
				return Enum.GetName(enumType, value);
#endif
			});
	    }

		public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
		{
#if DNX451
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