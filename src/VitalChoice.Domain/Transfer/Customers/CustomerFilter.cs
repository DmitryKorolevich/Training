using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Customers
{
    public class CustomerFilter : FilterBase
    {
        public string IdContains { get; set; }

        public bool IdAffiliateRequired { get; set; }

        public string IdAffiliate { get; set; }

        public string Company { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Address1 { get; set; }

		public string Address2 { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public string City { get; set; }

	    public string Country { get; set; }

	    public string State { get; set; }

		public string Zip { get; set; }
	}
}