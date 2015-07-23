﻿using System;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class VCustomer: Entity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateEdited { get; set; }

	    public RecordStatusCode StatusCode { get; set; }

	    public CustomerType IdObjectType { get; set; }

        public int? IdEditedBy { get; set; }

        public string CountryCode { get; set; }

        public string StateCode { get; set; }

	    public string StateName { get; set; }

	    public string CountryName { get; set; }

	    public string City { get; set; }

	    public string Company { get; set; }

	    public string Address1 { get; set; }

	    public string Address2 { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public string Zip { get; set; }

	    public string County { get; set; }

	    public string StateOrCounty { get; set; }
    }
}