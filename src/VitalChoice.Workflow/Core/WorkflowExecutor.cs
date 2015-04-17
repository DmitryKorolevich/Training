using System;
using System.Reflection;
using VitalChoice.Workflow.Attributes;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowExecutor<TContext, TResult> : IWorkflowExecutor<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        protected WorkflowExecutor(IWorkflowActionTree<TContext, TResult> workflowActionTree)
        {
            ActionTree = workflowActionTree;
            var executorAttribute = GetType().GetTypeInfo().GetCustomAttribute<WorkflowExecutorNameAttribute>();
            Name = executorAttribute?.Name;
        }

        public abstract TResult Execute(TContext context);

        public string Name { get; }

        protected IWorkflowActionTree<TContext, TResult> ActionTree { get; }
    }
}