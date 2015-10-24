using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowTree<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        Task<TResult> ExecuteAsync(TContext context);

        Task<TResult> ExecuteAsync(string actionName, TContext result);

        Task<TResult> ExecuteAsync<TAction>(TContext context) where TAction : IWorkflowExecutor<TContext, TResult>;

        IWorkflowExecutor<TContext, TResult> GetAction(string actionName);

        IWorkflowExecutor<TContext, TResult> GetAction<TAction>() where TAction : IWorkflowExecutor<TContext, TResult>;

        Task<TResult> GetActionResultAsync(string actionName, TContext context);

        bool TryGetActionResult(string actionName, TContext context, out TResult result);

        Task InitializeTreeAsync();
    }
}