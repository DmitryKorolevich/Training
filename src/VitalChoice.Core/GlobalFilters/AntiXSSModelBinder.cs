using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding.Metadata;

namespace VitalChoice.Core.GlobalFilters
{
    public class AntiXSSModelBinder : IModelBinder
    {
        private static readonly List<string> ForbiddenStrings = new List<string>
        {
            "'",
            //"&#39;",
            //"&#x27;",

            "\"",
            //"&quot;",
            //"&QUOT;",
            //"&#34;",
            //"&#x22;",

            "&",
            //"&amp;",
            //"&AMP;",
            //"&#38;",
            //"&#x26;",

            "<",
            //"&lt;",
            //"&LT;",
            //"&#60;",
            //"&#x3C;",

            ">",
            //"&gt;",
            //"&GT;",
            //"&#62;",
            //"&#x3E;",

            "/",
            //"&#47;",
            //"&#x2F;"
        };

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

                if (model != null)
                {
                    var metadata = bindingContext.ModelMetadata as DefaultModelMetadata;
                    var preventFilteringXSS = metadata?.Attributes.PropertyAttributes.Any(x => x is PreventXSSFilteringAttribute) ?? false;

                    
                    if (!preventFilteringXSS)
                    {
                        var containForbidden = modelAsString != null && ForbiddenStrings.Any(x => modelAsString.Contains(x));

                        if (containForbidden)
                        {
                            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Html forbidden");
                            return ModelBindingResult.SuccessAsync(bindingContext.ModelName, string.Empty);
                        }
                    }

                    return ModelBindingResult.SuccessAsync(bindingContext.ModelName, modelAsString);
                }
            }

            return ModelBindingResult.NoResultAsync;
        }
    }

    public class PreventXSSFilteringAttribute : Attribute
    {

    }
}
