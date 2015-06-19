using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.DynamicData.Delegates;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public abstract class ErrorBuilderBase<TProperty> : IDataContainer<TProperty>
    {
        protected ErrorBuilderBase(TProperty obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null)
        {
            Data = obj;
            CollectionName = collectionName;
            Indexes = indexes;
            PropertyName = propertyName;
            ErrorText = error;
        }

        protected readonly string CollectionName;
        protected string PropertyName;
        protected string ErrorText;
        protected readonly int[] Indexes;
        public TProperty Data { get; }

        public static string GetModelName(string dynamicName, Type modelType)
        {
            Dictionary<string, GenericProperty> cache;
            if (modelType != null && DynamicTypeCache.ModelTypeMappingCache.TryGetValue(modelType, out cache))
            {
                return cache.FirstOrDefault(m => (m.Value.Map.Name ?? m.Key) == dynamicName).Key;
            }
            return dynamicName;
        }
    }
}