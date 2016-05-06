using System;
using VitalChoice.Caching.Interfaces;

namespace VitalChoice.Caching.Services.Cache.Base
{
    internal class ContextTypeContainer : IContextTypeContainer
    {
        public Type[] ContextTypes { get; }

        public ContextTypeContainer(Type[] contextTypes)
        {
            ContextTypes = contextTypes;
        }
    }
}
