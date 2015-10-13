using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VC.Public.Validators;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.VitalGreen;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Public.Models
{
    public class VitalGreenStep2Model : BaseModel
    {
        public string Url { get; set; }

        public IList<VitalGreenDropoffLocation> Locations { get; set; }        
    }
}