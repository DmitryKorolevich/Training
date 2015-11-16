using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowFactory
    {
        Task<IWorkflowTree<TContext, TResult>> CreateTreeAsync<TContext, TResult>(string name)
            where TContext : WorkflowDataContext<TResult>;
    }
}