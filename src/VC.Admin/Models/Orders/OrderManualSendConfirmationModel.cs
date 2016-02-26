using VC.Admin.Models.Setting;
using VC.Admin.Validators.Customer;
using VC.Admin.Validators.Order;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Orders
{
	[ApiValidator(typeof(OrderManualSendConfirmationModelValidator))]
	public class OrderManualSendConfirmationModel : BaseModel
	{
		public OrderManualSendConfirmationModel()
		{
		}
        
	    public string Email { get; set; }
    }
}