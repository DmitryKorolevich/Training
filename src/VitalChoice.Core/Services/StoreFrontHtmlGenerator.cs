using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Antiforgery;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Localization;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.WebEncoders;

namespace VitalChoice.Core.Services
{
    public class StoreFrontHtmlGenerator : DefaultHtmlGenerator
    {
        public StoreFrontHtmlGenerator(IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor,
            IModelMetadataProvider metadataProvider, IUrlHelper urlHelper, IHtmlEncoder htmlEncoder)
            : base(antiforgery, optionsAccessor, metadataProvider, urlHelper, htmlEncoder)
        {
        }

        public override TagBuilder GenerateValidationSummary(ViewContext viewContext, bool excludePropertyErrors, string message,
            string headerTag,
            object htmlAttributes)
        {
            if (viewContext.ModelState.Any(s => string.IsNullOrWhiteSpace(s.Key)))
                return base.GenerateValidationSummary(viewContext, excludePropertyErrors, message, headerTag, htmlAttributes);
            return null;
        }
    }
}
