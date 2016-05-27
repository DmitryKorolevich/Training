using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Localization;

namespace VC.Public.DataAnnotations
{
    public class RequiredIfAttributeAdapter : AttributeAdapterBase<RequiredIfAttribute>
    {
        public RequiredIfAttributeAdapter(RequiredIfAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
                throw new ArgumentNullException(nameof(validationContext));
            string displayName = validationContext.ModelMetadata.GetDisplayName();
            string propertyDisplayName = RequiredIfWrapper.GetOtherPropertyDisplayName(validationContext, Attribute);
            ((RequiredIfWrapper) Attribute).ValidationContext = validationContext;
            return GetErrorMessage(validationContext.ModelMetadata, displayName, propertyDisplayName);
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            MergeAttribute(context.Attributes, "dependentproperty", Attribute.PropertyName);
            MergeAttribute(context.Attributes, "desiredvalue",
                Attribute.DesiredValue is bool ? Attribute.DesiredValue.ToString().ToLower() : Attribute.DesiredValue.ToString());
        }

        private sealed class RequiredIfWrapper : RequiredIfAttribute
        {
            public ModelValidationContextBase ValidationContext { get; set; }

            public RequiredIfWrapper(RequiredIfAttribute attribute)
                : base(attribute.PropertyName, attribute.DesiredValue)
            {
                if (string.IsNullOrEmpty(attribute.ErrorMessage) && string.IsNullOrEmpty(attribute.ErrorMessageResourceName) &&
                    !(attribute.ErrorMessageResourceType != null))
                    return;
                ErrorMessage = attribute.ErrorMessage;
                ErrorMessageResourceName = attribute.ErrorMessageResourceName;
                ErrorMessageResourceType = attribute.ErrorMessageResourceType;
            }

            public override string FormatErrorMessage(string name)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, new object[]
                {
                    ValidationContext.ModelMetadata.GetDisplayName(),
                    GetOtherPropertyDisplayName(ValidationContext, this)
                });
            }

            public static string GetOtherPropertyDisplayName(ModelValidationContextBase validationContext, RequiredIfAttribute attribute)
            {
                if (attribute.PropertyName == null && validationContext.ModelMetadata.ContainerType != null)
                {
                    ModelMetadata metadataForProperty =
                        validationContext.MetadataProvider.GetMetadataForProperty(validationContext.ModelMetadata.ContainerType,
                            attribute.PropertyName);
                    if (metadataForProperty != null)
                        return metadataForProperty.GetDisplayName();
                }
                return attribute.PropertyName;
            }
        }
    }

    public class RequiredIfAttribute : ValidationAttribute
	{
		public string PropertyName { get; }
        public object DesiredValue { get; }
		private readonly RequiredAttribute _innerAttribute;
        private readonly CallSite<Func<CallSite, object, object>> _dynamicParameter;

        public RequiredIfAttribute(string propertyName, object desiredvalue)
		{
			PropertyName = propertyName;
			DesiredValue = desiredvalue;
			_innerAttribute = new RequiredAttribute();
            CSharpArgumentInfo[] csharpArgumentInfoArray = new CSharpArgumentInfo[1];
            csharpArgumentInfoArray[0] = CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null);
            _dynamicParameter = CreateBinder(propertyName, csharpArgumentInfoArray);
		}

        private static CallSite<Func<CallSite, object, object>> CreateBinder(string model, CSharpArgumentInfo[] csharpArgumentInfoArray)
        {
            return
                CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, model, typeof(RequiredIfAttribute),
                    csharpArgumentInfoArray));
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var dependentValue = _dynamicParameter.Target(_dynamicParameter, value);

            if (dependentValue.Equals(DesiredValue))
			{
				if (!_innerAttribute.IsValid(value))
				{
					return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
				}
			}
			return ValidationResult.Success;
		}
	}
}
