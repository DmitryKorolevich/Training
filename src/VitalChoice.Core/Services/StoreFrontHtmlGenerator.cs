using System;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;

namespace VitalChoice.Core.Services
{
    public class StoreFrontHtmlGenerator : DefaultHtmlGenerator
    {
        public StoreFrontHtmlGenerator(IAntiforgery antiforgery, IOptions<MvcViewOptions> optionsAccessor,
            IModelMetadataProvider metadataProvider, IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder,
            ClientValidatorCache clientValidatorCache)
            : base(antiforgery, optionsAccessor, metadataProvider, urlHelperFactory, htmlEncoder, clientValidatorCache)
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