using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class AddUpdateBillingAddressModel: BaseModel
	{
	    public AddUpdateBillingAddressModel()
	    {
		    SendNews = true;
		    SendCatalog = true;
			BillingAddress = new BillingInfoModel();
			CreditCards = new Dictionary<int, string>();
	    }

	    [EmailAddress]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Required]
		[Map]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
		public string ConfirmPassword { get; set; }

	    public bool GuestCheckout { get; set; }

	    public bool SendNews { get; set; }

	    public bool SendCatalog { get; set; }

	    public Dictionary<int,string> CreditCards { get; set; }

	    public BillingInfoModel BillingAddress { get; set; }
	}
}
