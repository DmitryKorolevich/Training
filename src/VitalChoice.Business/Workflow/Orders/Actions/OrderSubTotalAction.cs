using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions
{
    public class OrderSubTotalAction: ComputableAction<OrderDataContext>
    {
        public OrderSubTotalAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
	        decimal discount = 0;
	        if (dataContext.DictionaryData.Keys.Contains("Discount"))
	        {
		        discount = dataContext.Data.Discount;
	        }
	  //      else if(dataContext.DictionaryData.Keys.Contains("AutoShip"))
	  //      {
			//	discounts = dataContext.Data.AutoShip;
			//}

            dataContext.DiscountTotal = - discount;
            dataContext.DiscountedSubtotal = dataContext.Data.PromoProducts + discount;
            dataContext.ShippingTotal = dataContext.StandardShippingCharges + dataContext.CanadaSurcharge +
                                    dataContext.AlaskaHawaiiSurcharge + dataContext.Data.ShippingUpgrade +
                                    dataContext.Data.ShippingOverride + dataContext.Data.SurchargeOverride;
            dataContext.TotalShipping = dataContext.StandardShippingCharges + dataContext.Data.ShippingUpgrade;
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}