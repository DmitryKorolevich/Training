using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Settings;

namespace VC.Admin.Models.Setting
{
    public class StateListItemModel : Model<State, IMode>
    {
        public int Id { get; set; }

        public string StateCode { get; set; }

        public string StateName { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public StateListItemModel(State item)
        {
            if(item!=null)
            {
                Id = item.Id;
                StateCode = item.StateCode;
                StateName = item.StateName;
                StatusCode = item.StatusCode;
            }
        }

        public override State Convert()
        {
            State state = new State();

            state.Id = Id;
            state.StateCode = StateCode;
            state.StateName = StateName;
            state.StatusCode = StatusCode;

            return state;
        }
    }
}