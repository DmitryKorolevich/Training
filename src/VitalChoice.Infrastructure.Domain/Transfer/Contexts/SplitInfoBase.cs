using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VitalChoice.Infrastructure.Domain.Transfer.Contexts
{
    public enum DeliveryServiceCostGroup
    {
        Free = 0,
        FirstCost = 1,
        SecondCost = 2,
        ThirdCost = 3,
        FourthCost = 4,
        FifthCost = 5,
        SixthCost = 6
    }

    public class SplitInfoBase<T>
        where T : ItemOrdered
    {
        protected readonly Func<IEnumerable<T>> GetSkus;

        public SplitInfoBase(Func<IEnumerable<T>> getSkus)
        {
            GetSkus = getSkus;
        }

        public virtual IEnumerable<T> GetPerishablePartProducts()
        {
            return GetSkus().Where(p => p.Sku.IdObjectType == (int) ProductType.Perishable);
        }

        public virtual IEnumerable<T> GetNonPerishablePartProducts()
        {
            return GetSkus().Where(p => p.Sku.IdObjectType != (int) ProductType.Perishable);
        }

        public decimal PerishableShippingOveridden { get; set; }

        public decimal NonPerishableShippingOverriden { get; set; }

        public decimal PerishableSurchargeOverriden { get; set; }

        public decimal NonPerishableSurchargeOverriden { get; set; }

        public decimal PerishableTax { get; set; }

        public decimal NonPerishableTax { get; set; }

        public decimal PerishableDiscount { get; set; }

        public decimal NonPerishableDiscount { get; set; }

        public decimal DiscountablePerishable { get; set; }

        public decimal DiscountableNonPerishable { get; set; }

        public DeliveryServiceCostGroup PerishableCostGroup { get; set; }

        public DeliveryServiceCostGroup NonPerishableCostGroup { get; set; }

        public bool ShouldSplit { get; set; }

        public virtual string GetCarrierDescription(ShippingUpgradeOption? serviceUpgrade, PreferredShipMethod serviceCarrier)
        {
            string result = string.Empty;
            switch (serviceCarrier)
            {
                case PreferredShipMethod.FedEx:
                    result += "FEDEX";
                    break;
                case PreferredShipMethod.UPS:
                    result += "UPS";
                    break;
                case PreferredShipMethod.USPS:
                    result += "USPS";
                    break;
                case PreferredShipMethod.OnTrac:
                    result += "ONTRAC";
                    break;
                default:
                    result += "BEST";
                    break;
            }
            switch (serviceUpgrade ?? ShippingUpgradeOption.None)
            {
                case ShippingUpgradeOption.Overnight:
                    result += " NEXT DAY STANDARD";
                    break;
                case ShippingUpgradeOption.SecondDay:
                    result += " 2ND DAY";
                    break;
                default:
                    result += " GROUND";
                    break;
            }
            return result;
        }

        public virtual string GetSwsCode(DeliveryServiceCostGroup costGroup, ShippingUpgradeOption? serviceUpgrade,
            PreferredShipMethod serviceCarrier)
        {
            string result;
            switch (costGroup)
            {
                case DeliveryServiceCostGroup.Free:
                    result = FreeGroup;
                    break;
                default:
                    result = ((int) costGroup).ToString(CultureInfo.InvariantCulture);
                    break;
            }
            switch (serviceUpgrade ?? ShippingUpgradeOption.None)
            {
                case ShippingUpgradeOption.Overnight:
                    result += SwsDelimeter + ServiceOvernightUpgrade;
                    break;
                case ShippingUpgradeOption.SecondDay:
                    result += SwsDelimeter + ServiceSecondDayUpgrade;
                    break;
            }
            switch (serviceCarrier)
            {
                case PreferredShipMethod.FedEx:
                    result += SwsDelimeter + ServiceCodeFedEx;
                    break;
                case PreferredShipMethod.UPS:
                    result += SwsDelimeter + ServiceCodeUps;
                    break;
                case PreferredShipMethod.USPS:
                    if ((serviceUpgrade ?? ShippingUpgradeOption.None) == ShippingUpgradeOption.None)
                        result += SwsDelimeter + ServiceCodeUsps;
                    break;
            }
            return result;
        }

        protected const string FreeGroup = "F";

        protected const string ServiceOvernightUpgrade = "O";
        protected const string ServiceSecondDayUpgrade = "2D";

        protected const string ServiceCodeUsps = "USPS";
        protected const string ServiceCodeUps = "UPS";
        protected const string ServiceCodeFedEx = "FE";

        protected const string SwsDelimeter = "_";
    }
}