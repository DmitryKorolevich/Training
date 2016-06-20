using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Workflow.Core
{
    public class WorkflowFactory : IWorkflowFactory
    {
        private readonly IActionItemProvider _actionItemProvider;
        private readonly ILifetimeScope _scope;
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();

        public WorkflowFactory(IActionItemProvider actionItemProvider, ILifetimeScope scope)
        {
            _actionItemProvider = actionItemProvider;
            _scope = scope;
        }

        public async Task<IWorkflowTreeExecutor<TContext, TResult>> CreateTreeAsync<TContext, TResult>(string name)
            where TContext : WorkflowDataContext<TResult>
        {
            object cachedResult;
            lock (Cache)
            {
                if (Cache.TryGetValue(name, out cachedResult))
                {
                    return new WorkflowTreeExecutor<TContext, TResult>((IWorkflowTree<TContext, TResult>) cachedResult, _scope);
                }
            }
            var treeInfo = await _actionItemProvider.GetTreeInfo(name);
            var result =
                (IWorkflowTree<TContext, TResult>) Activator.CreateInstance(treeInfo.TreeType, _actionItemProvider, name, treeInfo.IdTree);
            await result.InitializeTreeAsync();
            lock (Cache)
            {
                if (Cache.TryGetValue(name, out cachedResult))
                {
                    return new WorkflowTreeExecutor<TContext, TResult>((IWorkflowTree<TContext, TResult>) cachedResult, _scope);
                }
                Cache.Add(name, result);
            }
            return new WorkflowTreeExecutor<TContext, TResult>(result, _scope);
        }
    }
}