using System;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowExecutionContext : IDisposable
    {
        T Resolve<T>();
    }
}