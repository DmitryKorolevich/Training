using System;

namespace VitalChoice.Workflow.Core
{
    public interface ITreeContext : IDisposable
    {
        T Resolve<T>();
    }
}