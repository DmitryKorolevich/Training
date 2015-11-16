using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;
using VitalChoice.Validation.Models;

namespace VC.Public.Models
{
    public class VitalGreenStep2Model : BaseModel
    {
        public string Url { get; set; }

        public IList<VitalGreenDropoffLocation> Locations { get; set; }        
    }
}