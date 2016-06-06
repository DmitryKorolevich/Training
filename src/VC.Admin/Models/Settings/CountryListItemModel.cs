using System.Linq;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VC.Admin.Models.Setting
{
    public class CountryListItemModel : BaseModel
    {
        public int Id { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public ICollection<StateListItemModel> States { get; set; }

	    public CountryListItemModel()
	    {
		    States = new List<StateListItemModel>();
	    }

	    public CountryListItemModel(Country item)
        {
            if(item!=null)
            {
                Id = item.Id;
                CountryCode = item.CountryCode;
                CountryName = item.CountryName;
                StatusCode = item.StatusCode;

                States = item.States.Select(p => new StateListItemModel(p)).ToList();
            }
        }

        public Country Convert()
        {
            Country country = new Country();
            country.Id = Id;
            country.CountryCode = CountryCode;
            country.CountryName = CountryName;
            country.StatusCode = StatusCode;
            country.States = new List<State>();
            if(States!=null)
            {
                country.States = States.Select(p => p.Convert()).ToList();
            }

            return country;
        }
    }
}