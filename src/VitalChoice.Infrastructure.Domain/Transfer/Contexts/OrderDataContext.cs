using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Infrastructure.Domain.Transfer.Contexts
{
    public class SplitInfo
    {
        public int PerishableCount { get; set; }

        public decimal PerishableAmount { get; set; }

        public int NonPerishableCount { get; set; }

        public decimal NonPerishableAmount { get; set; }

        public int NonPerishableOrphanCount { get; set; }

        public int NonPerishableNonOrphanCount => NonPerishableCount - NonPerishableOrphanCount;

        public decimal PNpAmount => PerishableAmount + NonPerishableAmount;

        public bool SpecialSkuAdded { get; set; }

        public bool ThresholdReached { get; set; }

        public bool ShouldSplit { get; set; }

        public POrderType ProductTypes { get; set; }
    }

    public class OrderDataContext : ComputableDataContext
    {
        public Dictionary<string, int> Coutries { get; set; }

        public Dictionary<string, Dictionary<string, int>> States { get; set; }

        public Dictionary<int, Ecommerce.Domain.Entities.Addresses.Country> CoutryCodes { get; set; }

        public Dictionary<int, Dictionary<int, State>> StateCodes { get; set; }

        public List<PromotionDynamic> Promotions { get; set; }

        public OrderDataContext()
        {
            Messages = new List<MessageInfo>();
            PromoSkus = new List<SkuOrdered>();
            SplitInfo = new SplitInfo();
        }

        public OrderDynamic Order { get; set; }

        public decimal AlaskaHawaiiSurcharge { get; set; }

        public decimal CanadaSurcharge { get; set; }

        public decimal StandardShippingCharges { get; set; }

        public IList<LookupItem<ShippingUpgradeOption>> ShippingUpgradePOptions { get; set; }

        public IList<LookupItem<ShippingUpgradeOption>> ShippingUpgradeNpOptions { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TotalShipping { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public decimal ShippingOverride { get; set; }
        
        public decimal SurchargeOverride { get; set; }

        public bool FreeShipping { get; set; }

        public bool ProductsPerishableThresholdIssue { get; set; }

        public IList<SkuOrdered> SkuOrdereds { get; set; }

        public IList<SkuOrdered> PromoSkus { get; set; }

        public IList<MessageInfo> Messages { get; set; }

        public SplitInfo SplitInfo { get; set; }

        //public int GetCountryId(string countryCode)
        //{
        //    return _coutries.GetCountryId(countryCode);
        //}

        //public int GetStateId(string countryCode, string stateCode)
        //{
        //    return _states.GetStateId(countryCode, stateCode);
        //}

        public bool IsState(AddressDynamic address, string countryCode, string stateCode)
        {
            return States.GetStateId(countryCode, stateCode) == address.IdState;
        }

        public bool IsCountry(AddressDynamic address, string countryCode)
        {
            return Coutries.GetCountryId(countryCode) == address.IdCountry;
        }

        public string GetCountryCode(int idCountry)
        {
            return CoutryCodes.GetCountry(idCountry)?.CountryCode;
        }

        public string GetStateCode(int idCountry, int idState)
        {
            return StateCodes.GetState(idCountry, idState)?.StateCode;
        }

        public string GetCountryCode(AddressDynamic address)
        {
            return CoutryCodes.GetCountry(address.IdCountry)?.CountryCode;
        }

        public string GetRegionOrStateCode(AddressDynamic address)
        {
            return StateCodes.GetState(address.IdCountry, address.IdState ?? 0)?.StateCode ?? address.County;
        }
    }
}