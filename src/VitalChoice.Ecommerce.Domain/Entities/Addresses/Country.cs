using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.Addresses
{
	public class Country : Entity
	{
		public string CountryCode { get; set; }

		public string CountryName { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int Order { get; set; }

        public ICollection<State> States { get; set; }

	    public CustomerTypeCode IdVisibility { get; set; }
    }
}
