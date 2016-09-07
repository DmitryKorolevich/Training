using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class AddUpdateBillingAddressModel: BillingInfoModel
	{
	    public AddUpdateBillingAddressModel()
	    {
		    SendNews = true;
		    SendCatalog = false;
	    }

	    [EmailAddress]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Required]
		[Map]
		public string Email { get; set; }

		[Required]
        [AllowXss]
        public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
        [AllowXss]
        public string ConfirmPassword { get; set; }

	    public bool GuestCheckout { get; set; }

	    public bool SendNews { get; set; }

        public bool ShowSendCatalog { get; set; }

        public bool SendCatalog { get; set; }

        public int IdCustomerType { get; set; }
	}
}
