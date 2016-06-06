using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Workflow.Core
{
    public class WorkflowTreeExecutor<TContext, TResult> : IWorkflowTreeExecutor<TContext, TResult> where TContext : WorkflowDataContext<TResult>
    {
        private readonly IWorkflowTree<TContext, TResult> _tree;
        private readonly ILifetimeScope _scope;

        public WorkflowTreeExecutor(IWorkflowTree<TContext, TResult> tree, ILifetimeScope scope)
        {
            _tree = tree;
            _scope = scope;
        }

        public Task<TResult> ExecuteAsync(TContext context)
        {
            return _tree.ExecuteAsync(context, new TreeContext(_scope));
        }
    }
}
