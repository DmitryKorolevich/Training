﻿using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Checkout
{
    public class AddUpdateShippingMethodModel: ShippingInfoModel
	{
        [Map("GiftOrder")]
		public bool IsGiftOrder { get; set; }

		[Display(Name = "Gift Message")]
		[CustomMaxLength(250)]
        [Map]
		public string GiftMessage { get; set; }

		public bool UseBillingAddress { get; set; }

	    public bool SaveToProfile { get; set; }

	    public int? ShipAddressIdToOverride { get; set; }
	}
}
