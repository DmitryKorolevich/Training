using System;
using System.Linq;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Customer
{
	public class CustomerListItemModel : BaseModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string City { get; set; }

		public int? State { get; set; }

		public string County { get; set; }

		public DateTime UpdatedDate { get; set; }

		public string UpdateBy { get; set; }

		public CustomerListItemModel(CustomerDynamic item)
		{
			if (item != null)
			{
				Id = item.Id;

				var profileAddress = item.Addresses.Single(x => (AddressType)(x.IdObjectType ?? 0) == AddressType.Profile);

				Name = $"{profileAddress.Data.LastName}, {profileAddress.Data.FirstNam}";
				City = profileAddress.Data.City;
				State = profileAddress.IdState;
				County = profileAddress.County;
				UpdatedDate = item.DateEdited;
				//UpdateBy ?????
			}
		}
	}
}
