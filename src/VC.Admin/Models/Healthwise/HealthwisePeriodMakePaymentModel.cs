using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Healthwise
{
    public class HealthwisePeriodMakePaymentModel : BaseModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public bool PayAsGC { get; set; }
    }
}
