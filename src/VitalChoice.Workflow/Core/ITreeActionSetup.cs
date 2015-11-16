namespace VitalChoice.Workflow.Core
{
    public interface ITreeActionSetup<out TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        ITreeActionSetup<TContext, TResult> Action<T>()
            where T : IWorkflowExecutor<TContext, TResult>;
    }
}