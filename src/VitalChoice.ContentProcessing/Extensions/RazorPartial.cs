using System.IO;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ModelBinding.Metadata;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewEngines;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Templates.Attributes;
using Templates.Core;
using Templates.Data;
using Templates.Exceptions;
using VitalChoice.ContentProcessing.Extensions;

[assembly: ExportExtensions(typeof(RazorPartial))]

namespace VitalChoice.ContentProcessing.Extensions
{
    [ExtensionName("razor")]
    public class RazorPartial : AbstractExtension
    {
        private ICompositeViewEngine _viewEngine;

        public override ExType InitStart(InitContext initContext, ExType dataType, ExType chainedType, ExType parent)
        {
            var actionContext = initContext.Context.Options.Data as ActionContext;
            if (actionContext == null)
            {
                throw new TemplateProcessingException("ActionContext not found in options value");
            }
            _viewEngine = actionContext.HttpContext.ApplicationServices.GetRequiredService<ICompositeViewEngine>();
            return base.InitStart(initContext, parent, chainedType, null);
        }

        public override object ProcessData(object data, object chained, object parent)
        {
            var actionContext = chained as ActionContext;
            if (actionContext == null)
            {
                throw new TemplateProcessingException("ActionContext not found in chained data");
            }
            var result = _viewEngine.FindPartialView(actionContext, GetInnerResult(parent, chained));
            result.EnsureSuccessful();
            ViewDataDictionary viewData =
                new ViewDataDictionary(
                    new ViewDataDictionary(
                        new DefaultModelMetadataProvider(
                            actionContext.HttpContext.RequestServices.GetRequiredService<ICompositeMetadataDetailsProvider>()),
                        actionContext.ModelState), data);
            ITempDataDictionary tempData = actionContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionary>();
            using (var writer = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(actionContext, result.View, viewData, tempData, writer, new HtmlHelperOptions());
                result.View.RenderAsync(viewContext).Wait();
                writer.Flush();
                return writer.ToString();
            }
        }
    }
}