using VitalChoice.Validation.Models;

namespace VC.Public.Models.Lookup
{
    public class StateListItemModel : BaseModel
    {
        public int Id { get; set; }

		public string StateName { get; set; }
    }
}