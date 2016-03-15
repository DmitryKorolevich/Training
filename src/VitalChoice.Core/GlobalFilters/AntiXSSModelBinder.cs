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
                    var preventFilteringXSS = metadata?.Attributes.PropertyAttributes.Any(x => x.GetType() == typeof(PreventXSSFilteringAttribute)) ?? false;

                    var filteredString = FilterPotentiallyXSSEntries(modelAsString);
                    if (!preventFilteringXSS && filteredString != modelAsString)
                        bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Html forbidden");

                    return ModelBindingResult.SuccessAsync(bindingContext.ModelName, preventFilteringXSS ? modelAsString : filteredString);
                }
            }

            return ModelBindingResult.NoResultAsync;
        }

        private string FilterPotentiallyXSSEntries(string value)
        {
            return value.Replace("<", "").Replace(">", "").Replace("script", "");
        }
    }

    public class PreventXSSFilteringAttribute : Attribute
    {

    }
}
