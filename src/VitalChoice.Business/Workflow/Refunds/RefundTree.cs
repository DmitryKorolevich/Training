using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.Refunds.Actions;
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

        public override async Task<decimal> ExecuteAsync(OrderRefundDataContext context)
        {
            var total = await ExecuteAsync<RefundTotal>(context);
            context.Total = total;
            return total;
        }
    }
}