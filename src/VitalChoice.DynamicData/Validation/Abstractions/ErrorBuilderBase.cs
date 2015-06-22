using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        internal static string GetModelName(string dynamicName, Type modelType)
        {
            Dictionary<string, GenericProperty> cache;
            if (modelType != null && DynamicTypeCache.ModelTypeMappingCache.TryGetValue(modelType, out cache))
            {
                return cache.FirstOrDefault(m => (m.Value.Map.Name ?? m.Key) == dynamicName).Key;
            }
            return dynamicName;
        }

        internal static List<int> CompareCollections<T, TEntity, TPropertyResult>(ICollection<T> collection,
            Expression<Func<T, TPropertyResult>> propertySelector, ICollection<TEntity> values,
            Expression<Func<TEntity, TPropertyResult>> valueSelector)
        {
            var getter = propertySelector.Compile();
            var entityGetter = valueSelector.Compile();
            int index = 0;
            List<int> indexes = new List<int>();
            foreach (var item in collection)
            {
                foreach (var value in values)
                {
                    if (typeof (TPropertyResult) == typeof (string))
                    {
                        if (string.Equals((getter.Invoke(item) as string), (entityGetter.Invoke(value) as string),
                            StringComparison.OrdinalIgnoreCase))
                        {
                            indexes.Add(index);
                            break;
                        }
                    }
                    else
                    {
                        if (getter.Invoke(item)?.Equals(entityGetter.Invoke(value)) ?? false)
                        {
                            indexes.Add(index);
                            break;
                        }
                    }
                }
                index++;
            }
            return indexes;
        }
    }
}