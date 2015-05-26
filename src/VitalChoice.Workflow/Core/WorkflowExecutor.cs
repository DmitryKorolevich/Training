using System;
using System.Reflection;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowExecutor<TContext, TResult> : IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        protected WorkflowExecutor(IWorkflowTree<TContext, TResult> workflowTree, string actionName)
        {
            Tree = workflowTree;
            Name = actionName;
        }

        public abstract TResult Execute(TContext context);

        public string Name { get; }

        protected IWorkflowTree<TContext, TResult> Tree { get; }
    }
}