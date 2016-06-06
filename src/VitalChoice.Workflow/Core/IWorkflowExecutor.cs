using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowExecutor<in TContext, TResult>
        where TContext : WorkflowDataContext<TResult> {
        Task<TResult> ExecuteAsync(TContext context, ITreeContext executionContext);
        string Name { get; }
    }
}