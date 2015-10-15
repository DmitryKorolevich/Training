using System;
using VC.Admin.Models.Setting;
using VitalChoice.Domain.Entities;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Customer
{
	public class CustomerListItemModel : BaseModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string City { get; set; }

		public string Country { get; set; }

		public string State { get; set; }

		public DateTime DateEdited { get; set; }

		public string EditedBy { get; set; }

		public DateTime? LastOrderPlaced { get; set; }

		public int TotalOrders { get; set; }

		public int StatusCode { get; set; }
	}
}
