using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Helpers;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.ActionResolvers
{
    public enum SurchargeType
    {
        None = 0,
        AlaskaHawaii = 1,
        Canada = 2
    }

    public class ShippingSurchargeResolver : ComputableActionResolver<OrderContext>
    {
        public ShippingSurchargeResolver(IWorkflowTree<OrderContext, decimal> tree, string actionName)
            : base(tree, actionName)
        {
        }

        public override int GetActionKey(OrderContext context)
        {
            if (context.Order.ShippingAddress == null)
                return (int) SurchargeType.None;
            if (context.Order.ShippingAddress.IdState == context.States.GetStateId("us", "hi") ||
                context.Order.ShippingAddress.IdState == context.States.GetStateId("us", "ak"))
            {
                return (int) SurchargeType.AlaskaHawaii;
            }
            if (context.Order.ShippingAddress.IdCountry == context.Coutries.GetCountryId("ca"))
            {
                return (int) SurchargeType.Canada;
            }
            return (int) SurchargeType.None;
        }
    }
}