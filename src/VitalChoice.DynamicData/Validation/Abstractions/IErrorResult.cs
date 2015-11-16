using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.DynamicData.Validation.Abstractions
{
    public interface IErrorResult
    {
        IErrorResult Error(string error);

        List<MessageInfo> Build();
    }
}