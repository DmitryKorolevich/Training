using System.ComponentModel.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Checkout
{
    public class AddUpdateShippingMethodModel: ShippingInfoModel
	{
		[Display(Name = "Address Type")]
		public CheckoutAddressType AddressType { get; set; }

		[Display(Name = "Delivery Instructions")]
		[MaxLength(60)]
		public string DeliveryInstructions { get; set; }

		public bool IsGiftCertificate { get; set; }

		[Display(Name = "Gift Message")]
		[MaxLength(255)]
		public string GiftMessage { get; set; }

		public bool UseBillingAddress { get; set; }
	}
}
