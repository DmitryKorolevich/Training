using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds
{
    public class RefundTree : ComputableTree<OrderRefundDataContext>
    {
        public RefundTree(IActionItemProvider itemProvider, string treeName) : base(itemProvider, treeName)
        {
        }

        public override Task<decimal> ExecuteAsync(OrderRefundDataContext context)
        {
            throw new NotImplementedException();
        }
    }
}
