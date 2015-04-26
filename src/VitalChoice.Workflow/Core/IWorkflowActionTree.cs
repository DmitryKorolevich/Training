using System.Collections.Generic;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowActionTree<TContext, TResult> : IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        TResult Execute(string actionName, TContext result);

        TResult Execute<TAction>(TContext context) where TAction : IWorkflowExecutor<TContext, TResult>;

        IWorkflowExecutor<TContext, TResult> GetAction(string actionName);

        IWorkflowExecutor<TContext, TResult> GetAction<TAction>() where TAction : IWorkflowExecutor<TContext, TResult>;

        void SetUpActionDependencies(Dictionary<string, HashSet<string>> flatDependencyList);

        void SetUpActionResolverDependencies(Dictionary<string, Dictionary<int, string>> flatDependencyList);

        TResult GetActionResult(string actionName, TContext context);

        bool TryGetActionResult<TAction>(TContext context, out TResult result) where TAction : IWorkflowExecutor<TContext, TResult>;

        bool TryGetActionResult(string actionName, TContext context, out TResult result);
    }
}