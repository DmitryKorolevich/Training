namespace VitalChoice.Workflow.Core
{
    public interface IActionSetup<out TContext, TResult> 
        where TContext : WorkflowContext<TResult>
    {
        IActionSetup<TContext, TResult> Action<T>()
            where T : IWorkflowAction<TContext, TResult>;
        IActionSetup<TContext, TResult> ActionResolver<T>()
            where T : IWorkflowActionResolver<TContext, TResult>;
    }
}