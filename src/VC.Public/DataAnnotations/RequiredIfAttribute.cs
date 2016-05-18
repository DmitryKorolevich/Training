﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VC.Public.DataAnnotations
{
    public class RequiredIfAttribute : ValidationAttribute, IClientModelValidator
	{
		private string PropertyName { get; set; }
		private object DesiredValue { get; set; }
		private readonly RequiredAttribute _innerAttribute;

		public RequiredIfAttribute(string propertyName, object desiredvalue)
		{
			PropertyName = propertyName;
			DesiredValue = desiredvalue;
			_innerAttribute = new RequiredAttribute();
		}

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
#if !NETSTANDARD1_5
			var dependentValue = context.ObjectInstance.GetType().GetProperty(PropertyName).GetValue(context.ObjectInstance, null);

			if (dependentValue.Equals(DesiredValue))
			{
				if (!_innerAttribute.IsValid(value))
				{
					return new ValidationResult(FormatErrorMessage(context.DisplayName), new[] { context.MemberName });
				}
			}
#endif
			return ValidationResult.Success;
		}

        public void AddValidation(ClientModelValidationContext context)
        {
            //context.
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ClientModelValidationContext context)
	    {
		    var rule = new ModelClientValidationRule("requiredif", ErrorMessageString);
	    
			rule.ValidationParameters["dependentproperty"] = context.MetadataProvider.GetMetadataForProperty(context.ModelMetadata.ContainerType, PropertyName);
			rule.ValidationParameters["desiredvalue"] = DesiredValue is bool ? DesiredValue.ToString().ToLower() : DesiredValue;

			yield return rule;
		}
	}
}
