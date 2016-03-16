using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions
{
    public class OrderSubTotalAction: ComputableAction<OrderDataContext>
    {
        public OrderSubTotalAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
	        decimal discounts = 0;
	        if (dataContext.DictionaryData.Keys.Contains("Discount"))
	        {
		        discounts = dataContext.Data.Discount;
	        }
	        else if(dataContext.DictionaryData.Keys.Contains("AutoShip"))
	        {
				discounts = dataContext.Data.AutoShip;
			}

            dataContext.DiscountTotal = - discounts;
            dataContext.DiscountedSubtotal = dataContext.Data.Products + discounts;
            dataContext.ShippingTotal = dataContext.StandardShippingCharges + dataContext.CanadaSurcharge +
                                    dataContext.AlaskaHawaiiSurcharge + dataContext.Data.ShippingUpgrade +
                                    dataContext.Data.ShippingOverride + dataContext.Data.SurchargeOverride;
            dataContext.TotalShipping = dataContext.StandardShippingCharges + dataContext.Data.ShippingUpgrade;
            return Task.FromResult<decimal>(0);
        }
    }
}