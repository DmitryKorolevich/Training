using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VC.Admin.Models.Setting
{
    public class StateListItemModel : BaseModel
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

        public State Convert()
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