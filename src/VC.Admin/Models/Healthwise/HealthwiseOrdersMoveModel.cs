using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;

namespace VC.Admin.Models.Healthwise
{
    public class HealthwiseOrdersMoveModel
    {
        public int IdPeriod { get; set; }

        public ICollection<int> Ids { get; set; }
    }
}
