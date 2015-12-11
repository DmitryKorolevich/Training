﻿using System;
using System.ComponentModel;
using System.Reflection;

namespace Authorize.Net.Util
{
#pragma warning disable 1591
    public class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            var description = value.ToString();

#if DNX451 || NET451
            var fi = value.GetType().GetTypeInfo().GetDeclaredField(value.ToString());
            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
#endif

            return description;
        }
    }
#pragma warning restore 1591
}