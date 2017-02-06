using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
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
	    }
	}
}
