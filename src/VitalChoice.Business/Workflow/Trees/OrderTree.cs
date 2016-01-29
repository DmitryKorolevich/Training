using System.Linq;
using System.Threading.Tasks;
using Authorize.Net.CIM;
using VitalChoice.Business.Workflow.Actions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Trees
{
    public class OrderTree: ComputableTree<OrderDataContext>
    {
        public override async Task<decimal> ExecuteAsync(OrderDataContext dataContext)
        {
            if (!(dataContext.Order.Skus?.Any() ?? false))
                return 0;

            if (dataContext.Order.ShippingAddress == null)
            {
                dataContext.Order.ShippingAddress = dataContext.Order.Customer?.ShippingAddresses?.FirstOrDefault(s => (bool) s.Data.Default);
            }

            var result = await ExecuteAsync<TotalAction>(dataContext);
            dataContext.Total = result;

            return result;
        }

        public OrderTree(IActionItemProvider actionProvider, string treeName) : base(actionProvider, treeName)
        {
        }
    }
}