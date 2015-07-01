using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Customer
{
	public class PaymentMethodModel : Model<VitalChoice.Domain.Entities.eCommerce.Payment.PaymentMethod, AbstractModeContainer<IMode>>
	{
		public string Name { get; set; }

		public int Id { get; set; }
	}
}