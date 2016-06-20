using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.Orders.Actions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Reships
{
    public class ReshipTree : ComputableTree<OrderDataContext>
    {
        public ReshipTree(IActionItemProvider itemProvider, string treeName, int idTree) : base(itemProvider, treeName, idTree)
        {
        }

        public override async Task<decimal> ExecuteAsync(OrderDataContext context, ITreeContext treeContext)
        {
            if ((context.Order.Skus?.Count ?? 0) == 0)
                return 0;

            if (context.Order.ShippingAddress == null)
            {
                context.Order.ShippingAddress = context.Order.Customer?.ShippingAddresses?.FirstOrDefault(s => (bool) s.Data.Default);
            }

            var result = await ExecuteAsync<TotalAction>(context, treeContext);
            context.Total = result;

            return result;
        }
    }
}