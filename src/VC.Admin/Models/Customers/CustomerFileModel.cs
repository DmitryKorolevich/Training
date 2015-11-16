using System;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Customer
{
	public class CustomerFileModel : BaseModel
	{
		[Map]
		public int Id { get; set; }

		[Map]
		public string FileName { get; set; }

		[Map]
		public DateTime UploadDate { get; set; }

		[Map]
		public string Description { get; set; }
	}
}