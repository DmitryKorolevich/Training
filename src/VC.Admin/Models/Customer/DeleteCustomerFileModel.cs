using System;
using VC.Admin.Models.Setting;
using VC.Admin.Validators.Customer;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

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