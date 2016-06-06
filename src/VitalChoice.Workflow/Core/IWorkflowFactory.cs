using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowFactory
    {
        Task<IWorkflowTreeExecutor<TContext, TResult>> CreateTreeAsync<TContext, TResult>(string name)
            where TContext : WorkflowDataContext<TResult>;
    }
}