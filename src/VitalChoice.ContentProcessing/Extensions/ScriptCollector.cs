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

[assembly: ExportExtensions(typeof(ScriptCollector))]

namespace VitalChoice.ContentProcessing.Extensions
{
    [ExtensionName("script")]
    public class ScriptCollector : AbstractExtension
    {
        public override object ProcessData(ref Scope scope)
        {
            var viewContext = scope.CallerData as ContentViewContext;
            string scripts = GetInnerResult(ref scope);
            if (viewContext != null)
            {
                viewContext.AppendScript(scripts);
                //Consumed by collector
                return null;
            }
            return scripts;
        }
    }
}