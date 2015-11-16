using VC.Admin.Validators.Setting;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Models.Setting
{
    [ApiValidator(typeof(CountryManageModelValidator))]
    public class CountryManageModel : BaseModel
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

        public Country Convert()
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