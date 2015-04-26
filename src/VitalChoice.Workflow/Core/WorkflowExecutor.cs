using System;
using System.Reflection;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowExecutor<TContext, TResult> : IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        protected WorkflowExecutor(IWorkflowActionTree<TContext, TResult> workflowActionTree, string actionName)
        {
            ActionTree = workflowActionTree;
            Name = actionName;
        }

        public abstract TResult Execute(TContext context);

        public string Name { get; }

        protected IWorkflowActionTree<TContext, TResult> ActionTree { get; }
    }
}