using System;
using System.Collections.Generic;
using VitalChoice.DynamicData.Delegates;

namespace VitalChoice.DynamicData
{
    public static class DynamicTypeCache
    {
        internal static readonly Dictionary<Type, Dictionary<string, GenericProperty>> ModelTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();

        internal static readonly Dictionary<Type, Dictionary<string, GenericProperty>> DynamicTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();
    }
}
