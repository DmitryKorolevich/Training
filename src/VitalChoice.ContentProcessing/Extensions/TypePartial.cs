using System;
using System.IO;
using System.Linq;
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
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;
using System.Threading;

[assembly: ExportExtensions(typeof(TypePartial))]

namespace VitalChoice.ContentProcessing.Extensions
{
    [ExtensionName("type")]
    public class TypePartial : AbstractExtension
    {
        private string[] _namespaces;
        private Type _type;

        public override ExType InitStart(InitContext initContext, ExType dataType, ExType chainedType, ExType parent)
        {
            var contentViewContext = initContext.CompileScope.Options.Data as ContentViewContext;
            if (contentViewContext == null)
            {
                throw new TemplateProcessingException("ContentViewContext not found in options value");
            }
            _namespaces = initContext.CompileScope.Namespaces.ToArray();
            return new ExType(typeof(Type));
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
                if (scope.ModelData is string && !String.IsNullOrEmpty((string)scope.ModelData))
                {
                    if (_type == null)
                    {
                        Interlocked.CompareExchange(ref _type, ReflectionHelper.ResolveType((string)scope.ModelData, _namespaces), null);
                    }
                }
            }
            catch (Exception e)
            {
                throw new TemplateProcessingException(e.Message + "\r\n" + (e.InnerException?.Message ?? string.Empty));
            }
            return _type;
        }
    }
}