using System;
using System.Reflection;
using VitalChoice.Validation.Logic;

namespace VitalChoice.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ApiValidatorAttribute: Attribute
    {
        public Type ValidatorType { get; private set; }
        public ApiValidatorAttribute(Type validatorType)
        {
            if (validatorType == null)
                throw new ArgumentNullException("validatorType");
            var baseType = validatorType.GetTypeInfo().BaseType;
            while (baseType != null && (!baseType.GetTypeInfo().IsGenericType || !typeof(ModelValidator<>).IsAssignableFrom(baseType.GetGenericTypeDefinition()))) {
                baseType = baseType.GetTypeInfo().BaseType;
            }
            if (baseType == null || !typeof(ModelValidator<>).IsAssignableFrom(baseType.GetGenericTypeDefinition()))
            {
                throw new ArgumentException("validatorType");
            }
            ValidatorType = validatorType;
        }
    }
}