using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;
using VitalChoice.Infrastructure.Domain.Transfer.VitalGreen;

namespace VitalChoice.Interfaces.Services
{
    public interface IVitalGreenService
    {
        Task<VitalGreenReportModel> GetVitalGreenReport(DateTime date);

        Task<FedExZone> GetFedExZone(string stateCode);

        Task<FedExZone> GetFedExZone(int id);

        Task<VitalGreenRequest> InsertRequest(VitalGreenRequest request);
    }
}
