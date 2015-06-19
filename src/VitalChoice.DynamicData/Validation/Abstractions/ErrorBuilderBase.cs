using System;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public abstract class ErrorBuilderBase<TProperty> : IDataContainer<TProperty>
    {
        protected ErrorBuilderBase(TProperty obj, string collectionName = null, int[] indexes = null,
            string propertyName = null, string error = null)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
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
    }
}