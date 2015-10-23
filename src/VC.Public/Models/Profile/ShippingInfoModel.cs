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
    public class ShippingInfoModel : AddressModel
    {
		[EmailAddress]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Map]
		public string Email { get; set; }

		[Map]
	    public bool Default { get; set; }

		[Map]
	    public int Id { get; set; }
    }
}
