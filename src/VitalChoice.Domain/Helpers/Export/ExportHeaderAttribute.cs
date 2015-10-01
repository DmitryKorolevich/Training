using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Helpers.Export
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportHeaderAttribute : Attribute
    {
        public string HeaderText { get; private set; }

        public ExportHeaderAttribute(string headerText)
        {
            HeaderText = headerText;
        }
    }
}
