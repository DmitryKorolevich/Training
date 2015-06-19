using VitalChoice.Domain.Exceptions;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface IErrorResult<out TProperty>
    {
        IErrorResult<TProperty> Error(string error);

        MessageInfo[] Build();
    }
}