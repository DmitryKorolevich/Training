using System;
using VC.Admin.Models.Setting;
using VC.Admin.Validators.Customer;
using VitalChoice.Domain.Attributes;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

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