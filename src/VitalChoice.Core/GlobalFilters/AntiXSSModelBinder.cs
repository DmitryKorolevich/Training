using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Core.GlobalFilters
{
    public class AntiXssModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (!context.Metadata.IsComplexType)
                return new AntiXssModelBinder(context.Metadata.ModelType);
            return null;
        }
    }

    public class AntiXssModelBinder : IModelBinder
    {
        public AntiXssModelBinder(Type type)
        {
            _simpleTypeModelBinder = new SimpleTypeModelBinder(type);
        }

        private readonly SimpleTypeModelBinder _simpleTypeModelBinder;
        // < > " ' &
        // \u5F
        // \u{F9}
        // %FF
        private static readonly Regex ForbiddenStringsRegex =
            new Regex("[<>\"'&]|\\\\u[0-9A-F]{1,5}|\\\\u\\{[0-9A-F]{1,5}\\}|%[0-9A-F]{1,2}",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await _simpleTypeModelBinder.BindModelAsync(bindingContext);
            if (bindingContext?.Result.IsModelSet ?? false)
            {
                var modelAsString = bindingContext.Result.Model as string;
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
                        }
                    }
                }
            }
        }
    }

    public class AllowXssAttribute : Attribute
    {

    }
}