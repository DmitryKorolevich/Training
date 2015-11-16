using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Customer
{
	public class DeleteCustomerFileModel : BaseModel
	{
		[Map]
		public string FileName { get; set; }

		[Map]
		public string PublicId { get; set; }
	}
}