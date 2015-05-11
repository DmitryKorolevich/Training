namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowExecutor<in TContext, out TResult>
        where TContext : WorkflowContext<TResult> {
        TResult Execute(TContext context);
        string Name { get; }
    }
}