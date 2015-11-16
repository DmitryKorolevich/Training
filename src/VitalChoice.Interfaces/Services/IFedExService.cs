using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;

namespace VitalChoice.Interfaces.Services
{
    public interface IFedExService
    {
        string CreateLabel(VitalGreenRequest request, FedExZone zone);

        ICollection<VitalGreenDropoffLocation> GetDropoffLocations(VitalGreenRequest request);
    }
}
