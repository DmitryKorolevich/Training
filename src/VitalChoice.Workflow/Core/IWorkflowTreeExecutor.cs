using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowTreeExecutor<in TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        Task<TResult> ExecuteAsync(TContext context);
    }
}