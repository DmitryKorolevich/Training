﻿using System;
using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Profile
{
    public class BillingInfoModel: AddressModel
    {
		[Map]
		public int Id { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Name on Card")]
		[Map]
		public string NameOnCard { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.CREDIT_CARD_MAX_LENGTH)]
		[Display(Name = "Card Number")]
		[Map]
		public string CardNumber { get; set; }

		[Required]
		[Display(Name = "Month")]
		[Map]
		public int? ExpirationDateMonth { get; set; }

		[Required]
		[Display(Name = "Year")]
		[Map]
        public int? ExpirationDateYear { get; set; }

		[Required]
		[Display(Name = "Card Type")]
		[Map]
		public int CardType { get; set; }

        [Required]
        [Display(Name = "Security Code")]
        public string SecurityCode { get; set; }

		public bool IsSelected { get; set; }
	}
}
