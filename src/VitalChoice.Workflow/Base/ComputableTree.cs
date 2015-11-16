using System.Threading.Tasks;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableTree<TContext>: WorkflowTree<TContext, decimal> 
        where TContext : ComputableDataContext {
        protected ComputableTree(IActionItemProvider itemProvider, string treeName) : base(itemProvider, treeName) { }
        public abstract override Task<decimal> ExecuteAsync(TContext context);
    }
}