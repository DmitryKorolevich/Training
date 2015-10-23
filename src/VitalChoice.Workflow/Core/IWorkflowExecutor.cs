using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowExecutor<in TContext, TResult>
        where TContext : WorkflowDataContext<TResult> {
        Task<TResult> Execute(TContext context, IWorkflowExecutionContext executionContext);
        string Name { get; }
    }
}