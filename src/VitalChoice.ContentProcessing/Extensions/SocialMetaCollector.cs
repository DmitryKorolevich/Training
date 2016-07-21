using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Templates.Attributes;
using Templates.Core;
using Templates.Data;
using Templates.Strings;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Extensions;

[assembly: ExportExtensions(typeof(SocialMetaCollector))]

namespace VitalChoice.ContentProcessing.Extensions
{
    [ExtensionName("socialmeta")]
    public class SocialMetaCollector : AbstractExtension
    {
        public override object ProcessData(Scope scope)
        {
            var viewContext = scope.CallerData as ContentViewContext;
            string meta = GetInnerResult(scope);
            if (viewContext != null)
            {
                viewContext.AppendSocialMeta(meta);
                //Consumed by collector
                return null;
            }
            return meta;
        }
    }
}