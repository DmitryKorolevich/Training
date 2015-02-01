using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace VitalChoice.Validation.Validation
{
    public static class ValidatorsFactory
    {
        private static readonly Dictionary<Type, object> Cache;

        public static T GetValidator<T>()
            where T : class, new()
        {
            object result;
            if (!Cache.TryGetValue(typeof(T), out result))
            {
                result = new T();
                Cache.Add(typeof(T), result);
            }
            return (T)result;
        }

        public static ValidationResult Validate<T,TValue>(TValue value, string ruleSet = null)
            where T : AbstractValidator<TValue>, new()
        {
            return GetValidator<T>().Validate(value, ruleSet: ruleSet);
        }

        static ValidatorsFactory()
        {
            Cache = new Dictionary<Type, object>();
        }
    }
}