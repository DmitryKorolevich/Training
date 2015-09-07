namespace VitalChoice.Workflow.Core
{
    public interface IActionSetup<out TContext, in TResult> 
        where TContext : WorkflowContext<TResult>
    {
        IActionSetup<TContext, TResult> Dependency<T>()
            where T : IWorkflowExecutor<TContext, TResult>;
    }
}