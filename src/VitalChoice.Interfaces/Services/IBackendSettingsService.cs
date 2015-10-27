using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Entities.VitalGreen;
using VitalChoice.Domain.Transfer.VitalGreen;

namespace VitalChoice.Interfaces.Services
{
    public interface IBackendSettingsService
    {
        Country GetDefaultCountry();
    }
}
