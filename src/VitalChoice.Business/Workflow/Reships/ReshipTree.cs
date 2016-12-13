using VitalChoice.Business.Workflow.Orders;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Reships
{
    public class ReshipTree : OrderTree
    {
        public ReshipTree(IActionItemProvider itemProvider, string treeName, int idTree) : base(itemProvider, treeName, idTree)
        {
        }
    }
}