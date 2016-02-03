using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Profile
{
    public class ShippingInfoModel : AddressModel
    {
		[Map]
	    public bool Default { get; set; }

		[Map]
	    public int Id { get; set; }
    }
}
