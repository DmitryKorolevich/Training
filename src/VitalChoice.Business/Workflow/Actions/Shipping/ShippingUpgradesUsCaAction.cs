using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Shipping;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class ShippingUpgradesUsCaAction : ComputableAction<OrderContext>
    {
        public ShippingUpgradesUsCaAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            ShippingUpgradeOption? upgradeP = null;
            ShippingUpgradeOption? upgradeNp = null;
            if (context.Order.DictionaryData.ContainsKey("ShippingUpgradeP"))
            {
                upgradeP = (ShippingUpgradeOption?) context.Order.Data.ShippingUpgradeP;
            }
            if (context.Order.DictionaryData.ContainsKey("ShippingUpgradeNP"))
            {
                upgradeNp = (ShippingUpgradeOption?) context.Order.Data.ShippingUpgradeNP;
            }
            switch (context.SplitInfo.ProductTypes)
            {
                case POrderType.PNP:
                {
                    decimal upgradePerishableO;
                    if (context.SplitInfo.PerishableAmount > 165)
                        upgradePerishableO = context.SplitInfo.PerishableAmount*(decimal) 0.15;
                    else
                        upgradePerishableO = 25;

                    decimal upgradeNonPerishable2D;
                    if (context.SplitInfo.NonPerishableAmount > 100)
                        upgradeNonPerishable2D = context.SplitInfo.NonPerishableAmount*(decimal) 0.1;
                    else
                        upgradeNonPerishable2D = (decimal) 9.95;

                    decimal upgradeNonPerishableO;
                    if (context.SplitInfo.NonPerishableAmount > 165)
                        upgradeNonPerishableO = context.SplitInfo.NonPerishableAmount*(decimal) 0.15;
                    else
                        upgradeNonPerishableO = 25;
                    context.ShippingUpgradePOptions = new List<LookupItem<ShippingUpgradeOption>>
                    {
                        new LookupItem<ShippingUpgradeOption>
                        {
                            Key = ShippingUpgradeOption.Overnight,
                            Text = $"Overnight + {upgradePerishableO:C}"
                        }
                    };
                    context.ShippingUpgradeNpOptions = new List<LookupItem<ShippingUpgradeOption>>
                    {
                        new LookupItem<ShippingUpgradeOption>
                        {
                            Key = ShippingUpgradeOption.Overnight,
                            Text = $"Overnight + {upgradeNonPerishableO:C}"
                        },
                        new LookupItem<ShippingUpgradeOption>
                        {
                            Key = ShippingUpgradeOption.SecondDay,
                            Text = $"2nd Day + {upgradeNonPerishable2D:C}"
                        }
                    };
                    decimal result = 0;
                    if (upgradeP == ShippingUpgradeOption.Overnight)
                    {
                        result += upgradePerishableO;
                    }
                    if (upgradeNp == ShippingUpgradeOption.Overnight)
                    {
                        result += upgradeNonPerishableO;
                    }
                    else if (upgradeNp == ShippingUpgradeOption.SecondDay)
                    {
                        result += upgradeNonPerishable2D;
                    }
                    return result;
                }
                case POrderType.P:
                {
                    decimal upgradePerishableO;
                    if (context.SplitInfo.PNpAmount > 165)
                        upgradePerishableO = context.SplitInfo.PNpAmount*(decimal) 0.15;
                    else
                        upgradePerishableO = 25;

                    context.ShippingUpgradePOptions = new List<LookupItem<ShippingUpgradeOption>>
                    {
                        new LookupItem<ShippingUpgradeOption>
                        {
                            Key = ShippingUpgradeOption.Overnight,
                            Text = $"Overnight + {upgradePerishableO:C}"
                        }
                    };
                    decimal result = 0;
                    if (upgradeP == ShippingUpgradeOption.Overnight)
                    {
                        result += upgradePerishableO;
                    }
                    return result;
                }
                case POrderType.NP:
                {
                    decimal upgradeNonPerishable2D;
                    if (context.SplitInfo.NonPerishableAmount > 100)
                        upgradeNonPerishable2D = context.SplitInfo.NonPerishableAmount*(decimal) 0.1;
                    else
                        upgradeNonPerishable2D = (decimal) 9.95;

                    decimal upgradeNonPerishableO;
                    if (context.SplitInfo.NonPerishableAmount > 165)
                        upgradeNonPerishableO = context.SplitInfo.NonPerishableAmount*(decimal) 0.15;
                    else
                        upgradeNonPerishableO = 25;
                    context.ShippingUpgradeNpOptions = new List<LookupItem<ShippingUpgradeOption>>
                    {
                        new LookupItem<ShippingUpgradeOption>
                        {
                            Key = ShippingUpgradeOption.Overnight,
                            Text = $"Overnight + {upgradeNonPerishableO:C}"
                        },
                        new LookupItem<ShippingUpgradeOption>
                        {
                            Key = ShippingUpgradeOption.SecondDay,
                            Text = $"2nd Day + {upgradeNonPerishable2D:C}"
                        }
                    };
                    decimal result = 0;
                    if (upgradeNp == ShippingUpgradeOption.Overnight)
                    {
                        result += upgradeNonPerishableO;
                    }
                    else if (upgradeNp == ShippingUpgradeOption.SecondDay)
                    {
                        result += upgradeNonPerishable2D;
                    }
                    return result;
                }
                default:
                    return 0;
            }
        }
    }
}