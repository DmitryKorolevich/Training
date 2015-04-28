using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Validators.UserManagement;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validators.ContentManagement;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Validators.Setting;

namespace VitalChoice.Models.Setting
{
    [ApiValidator(typeof(StateManageModelValidator))]
    public class StateManageModel : Model<State, IMode>
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.StateCode)]
        public string StateCode { get; set; }

        [Localized(GeneralFieldNames.StateName)]
        public string StateName { get; set; }

        public string CountryCode { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Add { get; set; }

        public StateManageModel()
        {
        }

        public StateManageModel(State item)
        {
            Id = item.Id;
            StateCode = item.StateCode;
            StateName = item.StateName;
            CountryCode = item.CountryCode;
            StatusCode = item.StatusCode;
        }

        public override State Convert()
        {
            State toReturn = new State();

            toReturn.Id = Id;
            toReturn.StateCode = StateCode?.Trim();
            toReturn.StateName = StateName?.Trim();
            toReturn.CountryCode = CountryCode;
            toReturn.StatusCode = StatusCode;

            return toReturn;
        }
    }
}