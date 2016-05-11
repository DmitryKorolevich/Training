using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.Shipping
{
    public class ShippingUpgradesUsCaAction : ComputableAction<OrderDataContext>
    {
        public ShippingUpgradesUsCaAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            ShippingUpgradeOption? upgradeP = null;
            ShippingUpgradeOption? upgradeNp = null;
            if (dataContext.Order.DictionaryData.ContainsKey("ShippingUpgradeP"))
            {
                upgradeP = (ShippingUpgradeOption?) dataContext.Order.Data.ShippingUpgradeP;
            }
            if (dataContext.Order.DictionaryData.ContainsKey("ShippingUpgradeNP"))
            {
                upgradeNp = (ShippingUpgradeOption?) dataContext.Order.Data.ShippingUpgradeNP;
            }
            switch (dataContext.SplitInfo.ProductTypes)
            {
                case POrderType.PNP:
                {
                    decimal upgradePerishableO;
                    if (dataContext.SplitInfo.PerishableAmount > 165)
                        upgradePerishableO = Math.Round(dataContext.SplitInfo.PerishableAmount*(decimal) 0.15, 2);
                    else
                        upgradePerishableO = 25;

                    decimal upgradeNonPerishable2D;
                    if (dataContext.SplitInfo.NonPerishableAmount > 100)
                        upgradeNonPerishable2D = Math.Round(dataContext.SplitInfo.NonPerishableAmount*(decimal) 0.1, 2);
                    else
                        upgradeNonPerishable2D = (decimal) 9.95;

                    decimal upgradeNonPerishableO;
                    if (dataContext.SplitInfo.NonPerishableAmount > 165)
                        upgradeNonPerishableO = Math.Round(dataContext.SplitInfo.NonPerishableAmount*(decimal) 0.15, 2);
                    else
                        upgradeNonPerishableO = 25;
                    dataContext.ShippingUpgradePOptions = new List<LookupItem<ShippingUpgradeOption>>
                    {
                        new LookupItem<ShippingUpgradeOption>
                        {
                            Key = ShippingUpgradeOption.Overnight,
                            Text = $"Overnight + {upgradePerishableO:C}"
                        }
                    };
                    dataContext.ShippingUpgradeNpOptions = new List<LookupItem<ShippingUpgradeOption>>
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
                    return Task.FromResult(result);
                }
                case POrderType.P:
                {
                    decimal upgradePerishableO;
                    if (dataContext.SplitInfo.PNpAmount > 165)
                        upgradePerishableO = Math.Round(dataContext.SplitInfo.PNpAmount*(decimal) 0.15, 2);
                    else
                        upgradePerishableO = 25;

                    dataContext.ShippingUpgradePOptions = new List<LookupItem<ShippingUpgradeOption>>
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
                    return Task.FromResult(result);
                }
                case POrderType.NP:
                {
                    decimal upgradeNonPerishable2D;
                    if (dataContext.SplitInfo.NonPerishableAmount > 100)
                        upgradeNonPerishable2D = Math.Round(dataContext.SplitInfo.NonPerishableAmount*(decimal) 0.1, 2);
                    else
                        upgradeNonPerishable2D = (decimal) 9.95;

                    decimal upgradeNonPerishableO;
                    if (dataContext.SplitInfo.NonPerishableAmount > 165)
                        upgradeNonPerishableO = Math.Round(dataContext.SplitInfo.NonPerishableAmount*(decimal) 0.15, 2);
                    else
                        upgradeNonPerishableO = 25;
                    dataContext.ShippingUpgradeNpOptions = new List<LookupItem<ShippingUpgradeOption>>
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
                    return Task.FromResult(result);
                }
                default:
                    return Task.FromResult<decimal>(0);
            }
        }
    }
}