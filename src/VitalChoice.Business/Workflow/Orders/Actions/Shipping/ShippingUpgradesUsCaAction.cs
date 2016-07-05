using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
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

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            var upgradeP = (ShippingUpgradeOption?) context.Order.SafeData.ShippingUpgradeP ?? ShippingUpgradeOption.None;
            var upgradeNp = (ShippingUpgradeOption?) context.Order.SafeData.ShippingUpgradeNP ?? ShippingUpgradeOption.None;
            switch (context.SplitInfo.ProductTypes)
            {
                case POrderType.PNP:
                {
                    decimal upgradePerishableO;
                    if (context.SplitInfo.PerishableAmount > 165)
                        upgradePerishableO = Math.Round(context.SplitInfo.PerishableAmount*(decimal) 0.15, 2);
                    else
                        upgradePerishableO = 25;

                    decimal upgradeNonPerishable2D;
                    if (context.SplitInfo.NonPerishableAmount > 100)
                        upgradeNonPerishable2D = Math.Round(context.SplitInfo.NonPerishableAmount*(decimal) 0.1, 2);
                    else
                        upgradeNonPerishable2D = (decimal) 9.95;

                    decimal upgradeNonPerishableO;
                    if (context.SplitInfo.NonPerishableAmount > 165)
                        upgradeNonPerishableO = Math.Round(context.SplitInfo.NonPerishableAmount*(decimal) 0.15, 2);
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
                        context.SplitInfo.PerishableShippingOveridden += upgradePerishableO;
                    }
                    if (upgradeNp == ShippingUpgradeOption.Overnight)
                    {
                        result += upgradeNonPerishableO;
                        context.SplitInfo.NonPerishableSurchargeOverriden += upgradeNonPerishableO;
                    }
                    else if (upgradeNp == ShippingUpgradeOption.SecondDay)
                    {
                        result += upgradeNonPerishable2D;
                        context.SplitInfo.NonPerishableSurchargeOverriden += upgradeNonPerishable2D;
                    }
                    return Task.FromResult(result);
                }
                case POrderType.P:
                {
                    decimal upgradePerishableO;
                    if (context.SplitInfo.PNpAmount > 165)
                        upgradePerishableO = Math.Round(context.SplitInfo.PNpAmount*(decimal) 0.15, 2);
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
                    return Task.FromResult(result);
                }
                case POrderType.NP:
                {
                    decimal upgradeNonPerishable2D;
                    if (context.SplitInfo.NonPerishableAmount > 100)
                        upgradeNonPerishable2D = Math.Round(context.SplitInfo.NonPerishableAmount*(decimal) 0.1, 2);
                    else
                        upgradeNonPerishable2D = (decimal) 9.95;

                    decimal upgradeNonPerishableO;
                    if (context.SplitInfo.NonPerishableAmount > 165)
                        upgradeNonPerishableO = Math.Round(context.SplitInfo.NonPerishableAmount*(decimal) 0.15, 2);
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
                    return Task.FromResult(result);
                }
                default:
                    return TaskCache<decimal>.DefaultCompletedTask;
            }
        }
    }
}