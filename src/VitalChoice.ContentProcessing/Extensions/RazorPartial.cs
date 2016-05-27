using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Templates.Attributes;
using Templates.Core;
using Templates.Data;
using Templates.Exceptions;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;
using System.Collections.Generic;

[assembly: ExportExtensions(typeof(RazorPartial))]

namespace VitalChoice.ContentProcessing.Extensions
{
    [ExtensionName("razor")]
    public class RazorPartial : AbstractExtension
    {
        public override ExType InitStart(InitContext initContext, ExType dataType, ExType chainedType, ExType parent)
        {
            return base.InitStart(initContext, parent, chainedType, null);
        }

        public override object ProcessData(Scope scope)
        {
            var contentViewContext = scope.CallerData as ContentViewContext;
            if (contentViewContext == null)
            {
                throw new TemplateProcessingException("ContentViewContext not found caller data");
            }
            try
            {
                var viewEngine = contentViewContext.ActionContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
                var viewName = GetInnerResult(scope.Parent());
                var result = viewEngine.GetView(null, viewName, false);
                if (!result.Success)
                {
                    result = viewEngine.FindView(contentViewContext.ActionContext, viewName, false);
                }

                if (!result.Success)
                {
                    if (result.SearchedLocations.Any())
                    {
                        if (result.SearchedLocations.Any())
                        {
                            var locations = new List<string>(result.SearchedLocations);
                            locations.AddRange(result.SearchedLocations);
                            result = ViewEngineResult.NotFound(viewName, locations);
                        }
                    }
                }
                result.EnsureSuccessful(null);
                ViewDataDictionary viewData =
                    new ViewDataDictionary(
                        new ViewDataDictionary(
                            new DefaultModelMetadataProvider(
                                contentViewContext.ActionContext.HttpContext.RequestServices
                                    .GetRequiredService<ICompositeMetadataDetailsProvider>()),
                            contentViewContext.ActionContext.ModelState))
                    {
                        Model = scope.ModelData
                    };
                var tempDataFactory =
                    contentViewContext.ActionContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                using (var writer = new StringWriter())
                {
                    ViewContext viewContext = new ViewContext(contentViewContext.ActionContext, result.View, viewData,
                        tempDataFactory.GetTempData(contentViewContext.ActionContext.HttpContext), writer,
                        new HtmlHelperOptions());
                    result.View.RenderAsync(viewContext).Wait();
                    writer.Flush();
                    return writer.ToString();
                }
            }
            catch (Exception e)
            {
                return e.Message + "\r\n" + (e.InnerException?.Message ?? string.Empty);
            }
        }
    }
}