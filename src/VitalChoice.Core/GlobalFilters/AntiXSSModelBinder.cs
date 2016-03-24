using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding.Metadata;

namespace VitalChoice.Core.GlobalFilters
{
    public class AntiXSSModelBinder : IModelBinder
    {
        // < > " ' &
        // \u5F
        // \u{F9}
        // %FF
        private static readonly Regex ForbiddenStringsRegex = new Regex("^.*([<>\"'&]|\\\\u[0-9A-F]{2,5}|\\\\u\\{[0-9A-F]{2,5}\\}|%[0-9A-F]{2}).*$");

        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelMetadata.IsComplexType)
            {
                // this type cannot be converted
                return ModelBindingResult.NoResultAsync;
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                // no entry
                return ModelBindingResult.NoResultAsync;
            }

            var model = valueProviderResult.ConvertTo(bindingContext.ModelType);

            if (bindingContext.ModelType == typeof(string))
            {
                var modelAsString = model as string;

                if (modelAsString != null)
                {
                    var metadata = bindingContext.ModelMetadata as DefaultModelMetadata;
                    var preventFilteringXSS = metadata?.Attributes.PropertyAttributes?.Any(x => x is AllowXSSAttribute) ?? false;

                    
                    if (!preventFilteringXSS)
                    {
                        var containForbidden = ForbiddenStringsRegex.IsMatch(modelAsString);

                        if (containForbidden)
                        {
                            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "String contains characters that indicates potential injection threat");
                            return ModelBindingResult.NoResultAsync;
                        }
                    }

                    return ModelBindingResult.NoResultAsync;
                }
            }

            return ModelBindingResult.NoResultAsync;
        }
    }

    public class AllowXSSAttribute : Attribute
    {

    }
}
