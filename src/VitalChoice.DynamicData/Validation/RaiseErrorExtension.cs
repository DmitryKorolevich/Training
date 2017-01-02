﻿using System.Collections.Generic;
using System.Linq;
using VitalChoice.DynamicData.Validation.Abstractions;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.DynamicData.Validation
{
    public static class RaiseErrorExtension
    {
        public static IDynamicErrorBuilder<TDynamic> CreateError<TDynamic>(this TDynamic obj)
            where TDynamic : class, IModelType
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
            if (errors?.Count > 0)
                throw new AppValidationException(errors);
        }
        public static void Raise(this MessageInfo error)
        {
            if (error != null)
                throw new AppValidationException(error);
        }

        public static void Raise<T>(this ICollection<IErrorResult> results)
        {
            if (results?.Count > 0)
                throw new AppValidationException(results.Aggregate(Enumerable.Empty<MessageInfo>(),
                    (current, next) => current.Concat(next.Build())));
        }

        public static void Raise<T>(this IErrorResult result)
        {
            if (result != null)
                throw new AppValidationException(result.Build());
        }

        public static void Raise<T>(this ICollection<IErrorResult> results, string error)
        {
            if (results?.Count > 0)
                throw new AppValidationException(results.Aggregate(Enumerable.Empty<MessageInfo>(),
                    (current, next) => current.Concat(next.Error(error).Build())));
        }

        public static void Raise<T>(this IErrorResult result, string error)
        {
            if (result != null)
                throw new AppValidationException(result.Error(error).Build());
        }
    }
}