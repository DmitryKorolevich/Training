using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class AddUpdateBillingAddressModel: BillingInfoModel
	{
	    public AddUpdateBillingAddressModel()
	    {
		    SendNews = true;
		    SendCatalog = true;
	    }

	    [EmailAddress]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Required]
		[Map]
		public string Email { get; set; }

		[Required]
        [AllowXSS]
        public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
        [AllowXSS]
        public string ConfirmPassword { get; set; }

	    public bool GuestCheckout { get; set; }

	    public bool SendNews { get; set; }

	    public bool SendCatalog { get; set; }
	}
}
