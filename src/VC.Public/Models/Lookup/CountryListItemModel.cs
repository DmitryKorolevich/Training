﻿using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Lookup
{
    public class CountryListItemModel : BaseModel
    {
        public int Id { get; set; }

        public string CountryName { get; set; }

        public IList<StateListItemModel> States { get; set; }

        public CustomerTypeCode IdVisibility { get; set; }

        public CountryListItemModel()
	    {
		    States = new List<StateListItemModel>();
	    }
    }
}