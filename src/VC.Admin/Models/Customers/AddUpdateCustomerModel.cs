using System;
using System.Collections.Generic;
using VC.Admin.Models.Customers;
using VC.Admin.Validators.Customer;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

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
            CreditCards = new List<CreditCardModel>();
			Files = new List<CustomerFileModel>();
        }

		public bool IsConfirmed { get; set; }

		public Guid PublicUserId { get; set; }

        [Map]
        public int? IdAffiliate { get; set; }

        [Map]
		public int StatusCode { get; set; }

		[Map]
		public Guid PublicId { get; set; }

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
		public string Email { get; set; }

        public bool ProductReviewEmailEnabled { get; set; }

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

        [Map]
        public int? Source { get; set; }

        [Map]
        public string SourceDetails { get; set; }

		[Map("SuspensionReason")]
		public string Reason { get; set; }

		public AddressModel ProfileAddress { get; set; }

        public IList<AddressModel> Shipping { get; set; }

        public IList<CustomerNoteModel> CustomerNotes { get; set; }

        [DirectLocalized("Credit Card")]
        public IList<CreditCardModel> CreditCards { get; set; }
        
        [DirectLocalized("Check")]
        public CheckPaymentModel Check { get; set; }

        [DirectLocalized("On Approved Credit")]
        public OacPaymentModel Oac { get; set; }

        [DirectLocalized("Wire Transfer")]
        public WireTransferPaymentModel WireTransfer { get; set; }

        [DirectLocalized("Marketing")]
        public MarketingPaymentModel Marketing { get; set; }

        [DirectLocalized("VC Wellness Employee Program")]
        public VCWellnessEmployeeProgramPaymentModel VCWellness { get; set; }

        public IList<CustomerFileModel> Files { get; set; }
	}
}