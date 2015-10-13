﻿using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Lookup
{
    public class CountryListItemModel : BaseModel
    {
        public int Id { get; set; }

        public string CountryName { get; set; }

        public IList<StateListItemModel> States { get; set; }

	    public CountryListItemModel()
	    {
		    States = new List<StateListItemModel>();
	    }
    }
}