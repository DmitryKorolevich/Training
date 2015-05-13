using System;
using System.Linq;
using System.Collections.Generic;
using VC.Admin.Validators.Setting;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Domain.Entities.Settings;

namespace VC.Admin.Models.Setting
{
    [ApiValidator(typeof(CountryManageModelValidator))]
    public class CountryManageModel : Model<Country, IMode>
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.CountryCode)]
        public string CountryCode { get; set; }

        [Localized(GeneralFieldNames.CountryName)]
        public string CountryName { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Add { get; set; }

        public CountryManageModel()
        {
        }

        public CountryManageModel(Country item)
        {
            Id = item.Id;
            CountryCode = item.CountryCode;
            CountryName = item.CountryName;
            StatusCode = item.StatusCode;
        }

        public override Country Convert()
        {
            Country toReturn = new Country();

            toReturn.Id = Id;
            toReturn.CountryCode = CountryCode?.Trim();
            toReturn.CountryName = CountryName?.Trim();
            toReturn.StatusCode = StatusCode;

            return toReturn;
        }
    }
}