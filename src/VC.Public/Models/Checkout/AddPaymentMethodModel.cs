using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class AddPaymentMethodModel:BaseModel
	{
		[Required]
		[Display(Name = "Credit Card")]
		[Map]
		public int CardType { get; set; }

		[Required]
		[CustomMaxLength(BaseAppConstants.CREDIT_CARD_MAX_LENGTH)]
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
		[Display(Name = "Security Code")]
		public string SecurityCode { get; set; }

		[Required]
		[CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Name on Card")]
		[Map]
		public string NameOnCard { get; set; }
	}
}
