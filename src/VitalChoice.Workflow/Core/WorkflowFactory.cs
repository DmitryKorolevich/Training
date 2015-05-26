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

        public async Task<TTree> CreateTree<TTree, TContext, TResult>(string name)
            where TContext : WorkflowContext<TResult>
            where TTree : IWorkflowTree<TContext, TResult>
        {
            var result = (TTree) Activator.CreateInstance(typeof (TTree), _actionItemProvider, name);
            await result.InitializeTreeAsync();
            return result;
        }
    }
}
