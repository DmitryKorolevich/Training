using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Customer;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Customer
{
	[ApiValidator(typeof(CustomerAddUpdateModelValidator))]
	public class AddUpdateCustomerModel : BaseModel
	{
	    public AddUpdateCustomerModel()
	    {
			ApprovedPaymentMethods = new List<int>();
			OrderNotes = new List<int>();
			ProfileAddress = new AddressModel();
			Shipping = new List<AddressModel>();
			CustomerNotes = new List<CustomerNoteModel>();
        }

		[Map]
		public int Id { get; set; }

		[Map("IdObjectType")]
        public CustomerType CustomerType { get; set; }

		[Map]
		public TaxExempt? TaxExempt { get; set; }

		[Map]
		public string Website { get; set; }

		[Map]
		public string PromotingWebsites { get; set; }

		[Map]
		public Tier? Tier { get; set; }

		[Map]
		public int? TradeClass { get; set; }

		[Map]
		public DateTime? InceptionDate { get; set; }

		[Map]
		public string LinkedToAffiliate { get; set; }

		[Map]
		public string Email { get; set; }

		[Map]
		public string EmailConfirm { get; set; }

        [Map]
		public IList<int> ApprovedPaymentMethods { get; set; }

        [Map]
        public IList<int> OrderNotes { get; set; }

		[Map("IdDefaultPaymentMethod")]
		public int? DefaultPaymentMethod { get; set; }

		[Map]
		public bool DoNotMail { get; set; }

		[Map]
		public bool DoNotRent { get; set; }

		public bool SuspendUserAccount { get; set; }

		[Map("SuspensionReason")]
		public string Reason { get; set; }

		public AddressModel ProfileAddress { get; set; }

        public IList<AddressModel> Shipping { get; set; }

		public IList<CustomerNoteModel> CustomerNotes { get; set; }
	}
}