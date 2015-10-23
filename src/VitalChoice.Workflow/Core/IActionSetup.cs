namespace VitalChoice.Workflow.Core
{
    public interface IActionSetup<out TContext, TResult> 
        where TContext : WorkflowDataContext<TResult>
    {
        IActionSetup<TContext, TResult> Aggregate<T>()
            where T : IWorkflowExecutor<TContext, TResult>;

        IActionSetup<TContext, TResult> Dependency<T>()
            where T : IWorkflowExecutor<TContext, TResult>;
    }
}