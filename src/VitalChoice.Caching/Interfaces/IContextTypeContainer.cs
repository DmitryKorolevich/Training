using System;

namespace VitalChoice.Caching.Interfaces
{
    public interface IContextTypeContainer
    {
        Type[] ContextTypes { get; }
    }
}