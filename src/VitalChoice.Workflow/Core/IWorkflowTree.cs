using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowTree<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        Task<TResult> ExecuteAsync(TContext context, ITreeContext treeContext);

        Task<TResult> ExecuteAsync(string actionName, TContext result, ITreeContext treeContext);

        Task<TResult> ExecuteAsync<TAction>(TContext context, ITreeContext treeContext) where TAction : IWorkflowExecutor<TContext, TResult>;

        IWorkflowExecutor<TContext, TResult> GetAction(string actionName);

        IWorkflowExecutor<TContext, TResult> GetAction<TAction>() where TAction : IWorkflowExecutor<TContext, TResult>;

        Task<TResult> GetActionResultAsync(string actionName, TContext context, ITreeContext treeContext);

        bool TryGetActionResult(string actionName, TContext context, out TResult result);

        Task InitializeTreeAsync();
    }
}