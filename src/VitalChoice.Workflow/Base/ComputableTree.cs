using System.Threading.Tasks;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Base
{
    public abstract class ComputableTree<TContext>: WorkflowTree<TContext, decimal> 
        where TContext : ComputableDataContext {
        protected ComputableTree(IActionItemProvider itemProvider, string treeName, int idTree) : base(itemProvider, treeName, idTree) { }
        public abstract override Task<decimal> ExecuteAsync(TContext context, ITreeContext treeContext);
        }
}