using VitalChoice.Business.Workflow.Orders;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Export
{
    public class ExportTree : OrderTree
    {
        public ExportTree(IActionItemProvider actionProvider, string treeName, int idTree) : base(actionProvider, treeName, idTree)
        {
        }
    }
}