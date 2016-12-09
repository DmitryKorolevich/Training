using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Profile
{
    public class ShippingInfoModel : AddressModel
    {
		[Map]
	    public bool Default { get; set; }

		[Map]
	    public int Id { get; set; }

		[Map]
		[CustomMaxLength(60)]
		[Display(Name = "Delivery Instructions")]
		public string DeliveryInstructions { get; set; }

		[Map]
		[Display(Name = "Preferred Ship Method")]
		public PreferredShipMethod? PreferredShipMethod { get; set; }

		[Map("ShippingAddressType")]
		[Required]
		[Display(Name = "Address Type")]
		public ShippingAddressType? AddressType { get; set; }
	}
}
