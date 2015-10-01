﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Transfer.VitalGreen;

namespace VitalChoice.Interfaces.Services
{
    public interface IVitalGreenService
    {
        Task<VitalGreenReportModel> GetVitalGreenReport(DateTime date);
    }
}
