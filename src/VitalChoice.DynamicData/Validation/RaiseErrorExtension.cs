using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation.Abstractions;

namespace VitalChoice.DynamicData.Validation
{
    public static class RaiseErrorExtension
    {
        public static IDynamicErrorBuilder<TDynamic> CreateError<TDynamic>(this TDynamic obj)
            where TDynamic : class, IModelTypeContainer
        {
            return new DynamicErrorBuilder<TDynamic>(obj);
        }

        public static IErrorBuilder<T> CreateBasicError<T>(this T obj)
            where T : class
        {
            return new ErrorBuilder<T>(obj);
        }

        public static void Raise(this ICollection<MessageInfo> errors)
        {
            if (errors.Any())
                throw new AppValidationException(errors);
        }
        public static void Raise(this MessageInfo error)
        {
            throw new AppValidationException(error);
        }

        public static void Raise<T>(this ICollection<IErrorResult> results)
        {
            if (results.Any())
                throw new AppValidationException(results.Aggregate(Enumerable.Empty<MessageInfo>(),
                    (current, next) => current.Union(next.Build())));
        }

        public static void Raise<T>(this IErrorResult result)
        {
            throw new AppValidationException(result.Build());
        }

        public static void Raise<T>(this ICollection<IErrorResult> results, string error)
        {
            if (results.Any())
                throw new AppValidationException(results.Aggregate(Enumerable.Empty<MessageInfo>(),
                    (current, next) => current.Union(next.Error(error).Build())));
        }

        public static void Raise<T>(this IErrorResult result, string error)
        {
            throw new AppValidationException(result.Error(error).Build());
        }
    }
}