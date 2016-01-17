using System.ComponentModel.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Checkout
{
    public class AddUpdateShippingMethod: AddressModel
	{
		[Display(Name = "Address Type")]
		public CheckoutAddressType AddressType { get; set; }

		[Display(Name = "Delivery Instructions")]
		public string DeliveryInstructions { get; set; }

		[Display(Name = "Check here if this order is a gift")]
	    public bool IsGiftCertificate { get; set; }

		[Display(Name = "Delivery Instructions")]
		public string GiftMessage { get; set; }
	}
}
