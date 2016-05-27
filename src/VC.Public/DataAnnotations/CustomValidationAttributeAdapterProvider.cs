using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.Extensions.Localization;

namespace VC.Public.DataAnnotations
{
    public class CustomValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        protected readonly Dictionary<Type, Func<ValidationAttribute, IStringLocalizer, IAttributeAdapter>> AdapterConstructors =
            new Dictionary<Type, Func<ValidationAttribute, IStringLocalizer, IAttributeAdapter>>();

        public CustomValidationAttributeAdapterProvider()
        {
            AdapterConstructors.Add(typeof(RegularExpressionAttribute),
                (attribute, stringLocalizer) =>
                    new RegularExpressionAttributeAdapter((RegularExpressionAttribute) attribute, stringLocalizer));
            AdapterConstructors.Add(typeof(MaxLengthAttribute),
                (attribute, stringLocalizer) => new MaxLengthAttributeAdapter((MaxLengthAttribute) attribute, stringLocalizer));
            AdapterConstructors.Add(typeof(RequiredAttribute),
                (attribute, stringLocalizer) => new RequiredAttributeAdapter((RequiredAttribute) attribute, stringLocalizer));
            AdapterConstructors.Add(typeof(CompareAttribute),
                (attribute, stringLocalizer) => new CompareAttributeAdapter((CompareAttribute) attribute, stringLocalizer));
            AdapterConstructors.Add(typeof(MinLengthAttribute),
                (attribute, stringLocalizer) => new MinLengthAttributeAdapter((MinLengthAttribute) attribute, stringLocalizer));
            AdapterConstructors.Add(typeof(CreditCardAttribute),
                (attribute, stringLocalizer) =>
                    new DataTypeAttributeAdapter((DataTypeAttribute) attribute, "data-val-creditcard", stringLocalizer));
            AdapterConstructors.Add(typeof(StringLengthAttribute),
                (attribute, stringLocalizer) => new StringLengthAttributeAdapter((StringLengthAttribute) attribute, stringLocalizer));
            AdapterConstructors.Add(typeof(RangeAttribute),
                (attribute, stringLocalizer) => new RangeAttributeAdapter((RangeAttribute) attribute, stringLocalizer));
            AdapterConstructors.Add(typeof(EmailAddressAttribute),
                (attribute, stringLocalizer) =>
                    new DataTypeAttributeAdapter((DataTypeAttribute) attribute, "data-val-email", stringLocalizer));
            AdapterConstructors.Add(typeof(PhoneAttribute),
                (attribute, stringLocalizer) =>
                    new DataTypeAttributeAdapter((DataTypeAttribute) attribute, "data-val-phone", stringLocalizer));
            AdapterConstructors.Add(typeof(UrlAttribute),
                (attribute, stringLocalizer) =>
                    new DataTypeAttributeAdapter((DataTypeAttribute) attribute, "data-val-url", stringLocalizer));
            AdapterConstructors.Add(typeof(RequiredIfAttribute),
                (attribute, stringLocalizer) =>
                    new RequiredIfAttributeAdapter((RequiredIfAttribute) attribute, stringLocalizer));
            AdapterConstructors.Add(typeof(FutureDateAttribute),
                (attribute, stringLocalizer) => new FutureDateAttributeAdapter((FutureDateAttribute) attribute, stringLocalizer));
        }

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute,
            IStringLocalizer stringLocalizer)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            Type type = attribute.GetType();
            Func<ValidationAttribute, IStringLocalizer, IAttributeAdapter> constructor;
            if (AdapterConstructors.TryGetValue(type, out constructor))
            {
                return constructor(attribute, stringLocalizer);
            }
            return null;
        }
    }
}