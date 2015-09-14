namespace VitalChoice.Workflow.Core
{
    public interface IActionResolverSetup<out TContext, in TResult> 
        where TContext : WorkflowContext<TResult>
    {
        IActionResolverSetup<TContext, TResult> ResolvePath<T>(int key, string pathName)
            where T : IWorkflowExecutor<TContext, TResult>;

        IActionResolverSetup<TContext, TResult> Dependency<T>()
            where T : IWorkflowExecutor<TContext, TResult>;
    }
}