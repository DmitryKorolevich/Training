using System;
using System.Collections.Generic;
using VitalChoice.Caching.Interfaces;

namespace VitalChoice.Caching.Services.Cache.Base
{
    internal class ContextTypeContainer : IContextTypeContainer
    {
        private volatile HashSet<Type> _contextTypes;

        public HashSet<Type> ContextTypes
        {
            get { return _contextTypes; }
            set { _contextTypes = value; }
        }

        public ContextTypeContainer()
        {
            ContextTypes = new HashSet<Type>();
        }
    }
}
