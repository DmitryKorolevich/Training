﻿using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Trees
{
    public class OrderTree: ComputableTree<OrderDataContext>
    {
        public override async Task<decimal> ExecuteAsync(OrderDataContext dataContext)
        {
            if (dataContext.Order.Skus?.Any() ?? false)
            {
                var result = await ExecuteAsync<TotalAction>(dataContext);
                dataContext.Total = result;

                return result;
            }
            return 0;
        }

        public OrderTree(IActionItemProvider actionProvider, string treeName) : base(actionProvider, treeName)
        {
        }
    }
}