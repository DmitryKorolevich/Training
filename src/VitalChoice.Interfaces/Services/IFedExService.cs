using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.VitalGreen;

namespace VitalChoice.Interfaces.Services
{
    public interface IFedExService
    {
        string CreateLabel(VitalGreenRequest request, FedExZone zone);

        ICollection<VitalGreenDropoffLocation> GetDropoffLocations(VitalGreenRequest request);
    }
}
