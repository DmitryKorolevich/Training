﻿using System;
using VC.Admin.Models.Setting;
using VC.Admin.Validators.Customer;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models
{
	public class FileModel : BaseModel
	{
		public int Id { get; set; }

		public string FileName { get; set; }

		public DateTime UploadDate { get; set; }

		public string Description { get; set; }
	}
}