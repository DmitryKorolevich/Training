namespace VitalChoice.Workflow.Core
{
    public interface IActionResolverSetup<out TContext, TResult> 
        where TContext : WorkflowContext<TResult>
    {
        IActionResolverSetup<TContext, TResult> Action<T>(int key, string pathName)
            where T : IWorkflowAction<TContext, TResult>;
    }
}