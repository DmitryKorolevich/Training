using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Models.Setting
{
    public class CountryListItemModel : Model<Country, IMode>
    {
        public int Id { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public IEnumerable<StateListItemModel> States { get; set; }

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

        public override Country Convert()
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