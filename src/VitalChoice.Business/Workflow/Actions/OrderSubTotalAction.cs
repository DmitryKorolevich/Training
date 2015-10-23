using System.Threading.Tasks;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions
{
    public class OrderSubTotalAction: ComputableAction<OrderDataContext>
    {
        public OrderSubTotalAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteAction(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            dataContext.DiscountTotal = -dataContext.Data.Discount;
            dataContext.DiscountedSubtotal = dataContext.Data.Products + dataContext.Data.Discount;
            dataContext.ShippingTotal = dataContext.StandardShippingCharges + dataContext.CanadaSurcharge +
                                    dataContext.AlaskaHawaiiSurcharge + dataContext.Data.ShippingUpgrade +
                                    dataContext.Data.ShippingOverride + dataContext.Data.SurchargeOverride;
            dataContext.TotalShipping = dataContext.StandardShippingCharges + dataContext.Data.ShippingUpgrade;
            return Task.FromResult<decimal>(0);
        }
    }
}