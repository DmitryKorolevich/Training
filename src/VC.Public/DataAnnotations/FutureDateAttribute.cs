using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace VC.Public.DataAnnotations
{
    public class FutureDateAttributeAdapter : AttributeAdapterBase<FutureDateAttribute>
    {
        public FutureDateAttributeAdapter(FutureDateAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            if (validationContext == null)
                throw new ArgumentNullException(nameof(validationContext));
            string displayName = validationContext.ModelMetadata.GetDisplayName();
            ((FutureDateWrapper) Attribute).ValidationContext = validationContext;
            return GetErrorMessage(validationContext.ModelMetadata, displayName);
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
        }

        private sealed class FutureDateWrapper : FutureDateAttribute
        {
            public ModelValidationContextBase ValidationContext { get; set; }

            public FutureDateWrapper(FutureDateAttribute attribute)
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
                return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, ValidationContext.ModelMetadata.GetDisplayName(), name);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FutureDateAttribute : ValidationAttribute
    {
        public bool Disabled { get; set; }

        public override bool IsValid(object value)
        {
            if (!Disabled)
            {
                if (value == null)
                    return true;

                DateTime dt;
                if (DateTime.TryParse(value.ToString(), out dt))
                {
                    return dt.Date > DateTime.Today;
                }
            }

            return true;
        }
    }
}