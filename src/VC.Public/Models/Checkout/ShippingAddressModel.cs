using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class ShippingAddressModel : BaseAddressModel
    {
        public bool FromOrder { get; set; }

        public int? IdCustomerShippingAddress { get; set; }

        [Map]
        public int? Id { get; set; }

        [Map]
        public string DeliveryInstructions { get; set; }

        [Map]
        public PreferredShipMethod? PreferredShipMethod { get; set; }

        public string PreferredShipMethodName { get; set; }

        [Map("ShippingAddressType")]
        public ShippingAddressType? AddressType { get; set; }

        [Map("GiftOrder")]
        public bool IsGiftOrder { get; set; }

        [Map]
        public string GiftMessage { get; set; }

        public bool UseBillingAddress { get; set; }

        public bool SaveToProfile { get; set; }
    }
}
