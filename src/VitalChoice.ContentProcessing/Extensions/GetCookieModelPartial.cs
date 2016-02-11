﻿using System;
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
using System.Text;
using VitalChoice.Ecommerce.Utils;
using Microsoft.Extensions.Logging;

[assembly: ExportExtensions(typeof(GetCookieModelPartial))]

namespace VitalChoice.ContentProcessing.Extensions
{
    [ExtensionName("getcookiemodel")]
    [ChainedType(typeof(Type))]
    public class GetCookieModelPartial : AbstractExtension
    {
        public override ExType InitStart(InitContext initContext, ExType dataType, ExType chainedType, ExType parent)
        {
            var contentViewContext = initContext.Context.Options.Data as ContentViewContext;
            if (contentViewContext == null)
            {
                throw new TemplateProcessingException("ContentViewContext not found in options value");
            }
            return ExType.Dynamic;
        }

        public override object ProcessData(Scope scope)
        {
            object toReturn = null;
            var contentViewContext = scope.CallerData as ContentViewContext;
            if (contentViewContext == null)
            {
                throw new TemplateProcessingException("ContentViewContext not found caller data");
            }
            try
            {
                if (scope.ModelData is string && !String.IsNullOrEmpty((string)scope.ModelData) && scope.ChainedData!=null 
                    && scope.ChainedData is Type)
                {
                    var cookieName= (string)scope.ModelData;
                    var modelType = (Type)scope.ChainedData;
                    string cookies = contentViewContext.ActionContext.HttpContext.Request.Cookies[cookieName];
                    if (!string.IsNullOrEmpty(cookies))
                    {
                        try
                        {
                            cookies = Encoding.UTF8.GetString(Convert.FromBase64String(cookies));
                            var deserialized = cookies.FromJson(modelType);
                            toReturn = deserialized;
                        }
                        catch (Exception e)
                        {
                            ILogger logger =
                                contentViewContext.ActionContext.HttpContext.RequestServices.GetRequiredService<ILogger>();
                            logger.LogError(e.ToString());
                            contentViewContext.ActionContext.HttpContext.Response.Cookies.Delete(cookieName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new TemplateProcessingException(e.Message + "\r\n" + (e.InnerException?.Message ?? string.Empty));
            }
            return toReturn;
        }
    }
}