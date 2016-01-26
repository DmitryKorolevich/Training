﻿using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Auth
{
    public class RegisterWholesaleAccountModel : RegisterAccountModel
    {
		[Display(Name = "Trade Class")]
		[Map]
		public int? TradeClass { get; set; }

		[Display(Name = "Websites where you will promote our products(1 per line, max 4)")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXTAREA_FIELD_MAX_SIZE)]
        [Map]
		public string PromotingWebsites { get; set; }
	}
}