using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Core.GlobalFilters
{
    public class AntiXssModelBinderProvider : IModelBinderProvider
    {
        private readonly AntiXssModelBinder _binder = new AntiXssModelBinder();

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (!context.Metadata.IsComplexType)
                return _binder;
            return null;
        }
    }

    public class AntiXssModelBinder : IModelBinder
    {
        // < > " ' &
        // \u5F
        // \u{F9}
        // %FF
        private static readonly Regex ForbiddenStringsRegex =
            new Regex("[<>\"'&]|\\\\u[0-9A-F]{1,5}|\\\\u\\{[0-9A-F]{1,5}\\}|%[0-9A-F]{1,2}",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelMetadata.IsComplexType)
            {
                // this type cannot be converted
                return Task.Delay(0);
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                // no entry
                return Task.Delay(0);
            }

            var model = valueProviderResult.ConvertTo(bindingContext.ModelType);

            if (bindingContext.ModelType == typeof(string))
            {
                var modelAsString = model as string;

                if (modelAsString != null)
                {
                    var metadata = bindingContext.ModelMetadata as DefaultModelMetadata;
                    var preventFilteringXss = metadata?.Attributes.PropertyAttributes?.Any(x => x is AllowXssAttribute) ?? false;


                    if (!preventFilteringXss)
                    {
                        var containForbidden = ForbiddenStringsRegex.IsMatch(modelAsString);

                        if (containForbidden)
                        {
                            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid characters used");
                            return Task.Delay(0);
                        }
                    }

                    return Task.Delay(0);
                }
            }

            return Task.Delay(0);
        }
    }

    public class AllowXssAttribute : Attribute
    {

    }
}