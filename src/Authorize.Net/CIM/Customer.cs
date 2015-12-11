using System.Collections.Generic;
using Authorize.Net.Utility;

namespace Authorize.Net.CIM
{
    public enum ValidationMode
    {
        None,
        TestMode,
        LiveMode
    }

    /// <summary>
    ///     This is an abstraction for use with the CIM API. It's a partial class so you can combine it with your class as
    ///     needed.
    /// </summary>
    public class Customer
    {
        public Customer()
        {
            ID = "MerchantCustomerID";
            ShippingAddresses = new List<Address>();
            PaymentProfiles = new List<PaymentProfile>();
        }

        public string ID { get; set; }
        public string ProfileID { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public Address BillingAddress { get; set; }
        public IList<Address> ShippingAddresses { get; set; }
        public IList<PaymentProfile> PaymentProfiles { get; set; }

        internal static validationModeEnum ToValidationMode(ValidationMode mode)
        {
            switch (mode)
            {
                case ValidationMode.None:
                    return validationModeEnum.none;
                case ValidationMode.TestMode:
                    return validationModeEnum.testMode;
                case ValidationMode.LiveMode:
                    return validationModeEnum.liveMode;
                default:
                    return (validationModeEnum) mode;
            }
        }
    }
}