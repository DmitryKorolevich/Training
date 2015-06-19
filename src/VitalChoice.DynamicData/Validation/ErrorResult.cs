using System.Linq;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Validation.Abstractions;
using VitalChoice.Validation.Helpers;

namespace VitalChoice.DynamicData.Validation
{
    public class ErrorResult<TProperty> : ErrorBuilderBase<TProperty>, IErrorResult<TProperty>
    {
        public ErrorResult(string collectionName = null, int[] indexes = null, string propertyName = null,
            string error = null) : base(default(TProperty), collectionName, indexes, propertyName, error)
        {
        }

        public IErrorResult<TProperty> Error(string error)
        {
            ErrorText = error;
            return this;
        }

        public MessageInfo[] Build()
        {
            if (!string.IsNullOrEmpty(CollectionName))
            {
                return Indexes.Select(i => new MessageInfo
                {
                    Field = CollectionFormProperty.GetFullName(CollectionName, i, PropertyName),
                    Message = ErrorText
                }).ToArray();
            }
            return new[]
            {
                new MessageInfo
                {
                    Field = PropertyName,
                    Message = ErrorText
                }
            };
        }
    }
}