using System.ComponentModel.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Checkout
{
    public class AddUpdateShippingMethodModel: AddressModel
	{
		[Display(Name = "Address Type")]
		public CheckoutAddressType AddressType { get; set; }

		[Display(Name = "Delivery Instructions")]
		public string DeliveryInstructions { get; set; }

		public bool IsGiftCertificate { get; set; }

		[Display(Name = "Gift Message")]
		public string GiftMessage { get; set; }

		public bool UseBillingAddress { get; set; }
	}
}
