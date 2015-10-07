using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Helpers;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Shipping;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Workflow.Contexts
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

    public class OrderContext : ComputableContext
    {
        private readonly Dictionary<string, int> _coutries;

        private readonly Dictionary<string, Dictionary<string, int>> _states;

        private readonly Dictionary<int, Country> _coutryCodes;

        private readonly Dictionary<int, Dictionary<int, State>> _stateCodes;

        public OrderContext(ICollection<Country> coutries)
        {
            _coutries = coutries.ToDictionary(c => c.CountryCode, c => c.Id, StringComparer.OrdinalIgnoreCase);
            _states = coutries.ToDictionary(c => c.CountryCode,
                c => c.States.ToDictionary(s => s.StateCode, s => s.Id, StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
            _coutryCodes = coutries.ToDictionary(c => c.Id);
            _stateCodes = coutries.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
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
            return _states.GetStateId(countryCode, stateCode) == address.IdState;
        }

        public bool IsCountry(AddressDynamic address, string countryCode)
        {
            return _coutries.GetCountryId(countryCode) == address.IdCountry;
        }

        public string GetCountryCode(int idCountry)
        {
            return _coutryCodes.GetCountry(idCountry)?.CountryCode;
        }

        public string GetStateCode(int idCountry, int idState)
        {
            return _stateCodes.GetState(idCountry, idState)?.StateCode;
        }

        public string GetCountryCode(AddressDynamic address)
        {
            return _coutryCodes.GetCountry(address.IdCountry)?.CountryCode;
        }

        public string GetRegionOrStateCode(AddressDynamic address)
        {
            return _stateCodes.GetState(address.IdCountry, address.IdState ?? 0)?.StateCode ?? address.County;
        }
    }
}