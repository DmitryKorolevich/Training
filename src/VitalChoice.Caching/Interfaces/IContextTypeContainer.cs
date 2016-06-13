using System;
using System.Collections.Generic;

namespace VitalChoice.Caching.Interfaces
{
    public interface IContextTypeContainer
    {
        HashSet<Type> ContextTypes { get; set; }
    }
}