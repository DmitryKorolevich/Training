using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Attributes;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Payment;

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

		public bool IsSelected { get; set; }
	}
}
