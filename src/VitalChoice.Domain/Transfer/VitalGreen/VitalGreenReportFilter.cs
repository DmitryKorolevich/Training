using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.VitalGreen
{
    public class VitalGreenReportFilter : FilterBase
    {
        public int Year { get; set; }

        public int Month { get; set; }
    }
}
