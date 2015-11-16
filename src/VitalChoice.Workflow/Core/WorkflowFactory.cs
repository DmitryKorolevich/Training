using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public class WorkflowFactory : IWorkflowFactory
    {
        private readonly IActionItemProvider _actionItemProvider;
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();

        public WorkflowFactory(IActionItemProvider actionItemProvider)
        {
            _actionItemProvider = actionItemProvider;
        }

        public async Task<IWorkflowTree<TContext, TResult>> CreateTreeAsync<TContext, TResult>(string name)
            where TContext : WorkflowDataContext<TResult>
        {
            object cachedResult;
            lock (Cache)
            {
                if (Cache.TryGetValue(name, out cachedResult))
                {
                    return (IWorkflowTree<TContext, TResult>) cachedResult;
                }
            }
            var treeType = await _actionItemProvider.GetTreeType(name);
            var result = (IWorkflowTree<TContext, TResult>)Activator.CreateInstance(treeType, _actionItemProvider, name);
            await result.InitializeTreeAsync();
            lock (Cache)
            {
                if (Cache.TryGetValue(name, out cachedResult))
                {
                    return (IWorkflowTree<TContext, TResult>)cachedResult;
                }
                Cache.Add(name, result);
            }
            return result;
        }
    }
}