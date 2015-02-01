using System;
using System.Linq;
using System.Reflection;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Controllers
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ControlModeAttribute: Attribute
    {
        public object Mode { get; private set; }
        public Type ViewModeType { get; private set; }

        public ControlModeAttribute(object mode, Type viewMode)
        {
            if (viewMode.GetTypeInfo().ImplementedInterfaces.All(i => i != typeof(IMode)))
                throw new ArgumentException("viewMode Type should implement IMode");
            Mode = mode;
            ViewModeType = viewMode;
        }
    }
}
