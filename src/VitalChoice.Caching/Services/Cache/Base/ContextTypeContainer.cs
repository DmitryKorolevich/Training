using System;
using System.Collections.Generic;
using VitalChoice.Caching.Interfaces;

namespace VitalChoice.Caching.Services.Cache.Base
{
    internal class ContextTypeContainer : IContextTypeContainer
    {
        public HashSet<Type> ContextTypes { get; set; }

        public ContextTypeContainer()
        {
            ContextTypes = new HashSet<Type>();
        }
    }
}
