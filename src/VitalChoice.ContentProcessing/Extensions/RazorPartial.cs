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

[assembly: ExportExtensions(typeof(RazorPartial))]

namespace VitalChoice.ContentProcessing.Extensions
{
    [ExtensionName("razor")]
    public class RazorPartial : AbstractExtension
    {
        private ICompositeViewEngine _viewEngine;

        public override ExType InitStart(InitContext initContext, ExType dataType, ExType chainedType, ExType parent)
        {
            var contentViewContext = initContext.CompileScope.Options.Data as ContentViewContext;
            if (contentViewContext == null)
            {
                throw new TemplateProcessingException("ContentViewContext not found in options value");
            }
            _viewEngine = contentViewContext.ActionContext.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
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
                var result = _viewEngine.FindView(contentViewContext.ActionContext, GetInnerResult(scope.Parent()), false);
                result.EnsureSuccessful(Enumerable.Empty<string>());
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
                //ITempDataDictionary tempData =
                //    contentViewContext.ActionContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionary>();
                using (var writer = new StringWriter())
                {
                    ViewContext viewContext = new ViewContext(contentViewContext.ActionContext, result.View, viewData, null, writer,
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