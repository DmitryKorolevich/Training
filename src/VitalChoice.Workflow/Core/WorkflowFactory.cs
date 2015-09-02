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

        public WorkflowFactory(IActionItemProvider actionItemProvider)
        {
            _actionItemProvider = actionItemProvider;
        }

        public async Task<IWorkflowTree<TContext, TResult>> CreateTree<TContext, TResult>(string name)
            where TContext : WorkflowContext<TResult>
        {
            var treeType = await _actionItemProvider.GetTreeType(name);
            var result = (IWorkflowTree<TContext, TResult>)Activator.CreateInstance(treeType, _actionItemProvider, name);
            await result.InitializeTreeAsync();
            return result;
        }
    }
}