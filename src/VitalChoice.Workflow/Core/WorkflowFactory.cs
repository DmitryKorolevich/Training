using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Workflow;

namespace VitalChoice.Workflow.Core
{
    public class WorkflowFactory : IWorkflowFactory
    {
        private readonly IActionItemProvider _actionItemProvider;
        private readonly Dictionary<string, object> _cache;

        public WorkflowFactory(IActionItemProvider actionItemProvider)
        {
            _cache = new Dictionary<string, object>();
            _actionItemProvider = actionItemProvider;
        }

        public async Task<IWorkflowTree<TContext, TResult>> CreateTreeAsync<TContext, TResult>(string name)
            where TContext : WorkflowContext<TResult>
        {
            object cachedResult;
            if (_cache.TryGetValue(name, out cachedResult))
            {
                return (IWorkflowTree<TContext, TResult>) cachedResult;
            }
            var treeType = await _actionItemProvider.GetTreeType(name);
            var result = (IWorkflowTree<TContext, TResult>)Activator.CreateInstance(treeType, _actionItemProvider, name);
            await result.InitializeTreeAsync();
            _cache.Add(name, result);
            return result;
        }
    }
}