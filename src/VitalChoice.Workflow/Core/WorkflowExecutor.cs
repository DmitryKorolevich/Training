using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowExecutor<TContext, TResult> : IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        protected WorkflowExecutor(IWorkflowTree<TContext, TResult> workflowTree, string actionName)
        {
            Tree = workflowTree;
            Name = actionName;
        }

        public abstract Task<TResult> ExecuteAsync(TContext context, IWorkflowExecutionContext executionContext);

        public string Name { get; }

        protected IWorkflowTree<TContext, TResult> Tree { get; }
    }
}