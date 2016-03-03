using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;
using VitalChoice.Infrastructure.Domain.Transfer.VitalGreen;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class VitalGreenService : IVitalGreenService
    {
        private readonly IRepositoryAsync<FedExZone> _fedExZoneRepository;
        private readonly IRepositoryAsync<VitalGreenRequest> _vitalGreenRequestRepository;

        public VitalGreenService(
            IRepositoryAsync<FedExZone> fedExZoneRepository,
            IRepositoryAsync<VitalGreenRequest> vitalGreenRequestRepository)
        {
            _fedExZoneRepository = fedExZoneRepository;
            _vitalGreenRequestRepository = vitalGreenRequestRepository;
        }

        public async Task<VitalGreenReportModel> GetVitalGreenReport(DateTime start)
        {
            VitalGreenReportModel toReturn = new VitalGreenReportModel();
            toReturn.Dates = new List<VitalGreenReportDateStatisticModel>();
            toReturn.Zones = new List<VitalGreenReportZoneStatisticModel>();
            DateTime end = start.AddMonths(1);

            var zones = (await _fedExZoneRepository.Query().SelectAsync(false)).ToList();
            var requests = (await _vitalGreenRequestRepository.Query(p=>p.DateCompleted>=start && p.DateCompleted<=end).SelectAsync(false)).ToList();
            foreach(var request in requests)
            {
                request.Zone = zones.FirstOrDefault(p => request.ZoneId == p.Id);
            }

            toReturn.CompletedCount = requests.Count();
            foreach(var zone in zones)
            {
                VitalGreenReportZoneStatisticModel model = new VitalGreenReportZoneStatisticModel();
                model.Zone = zone;
                model.CompletedCount = requests.Count(p => p.ZoneId == zone.Id);
                toReturn.Zones.Add(model);
            }

            var currentDate = start;
            while(currentDate<end)
            {
                var newCurrentDate = currentDate.AddDays(1);
                VitalGreenReportDateStatisticModel model = new VitalGreenReportDateStatisticModel();
                model.Date = currentDate;
                model.Requests = requests.Where(p => p.DateCompleted >= currentDate && p.DateCompleted <= newCurrentDate).ToList();
                model.CompletedCount = model.Requests.Count();
                toReturn.Dates.Add(model);
                currentDate = newCurrentDate;
            }

            return toReturn;
        }

        public async Task<FedExZone> GetFedExZone(string stateCode)
        {
            var zone = (await _fedExZoneRepository.Query(p=>p.StatesCovered.Contains(stateCode)).SelectAsync(false)).FirstOrDefault();
            return zone;
        }

        public async Task<FedExZone> GetFedExZone(int id)
        {
            var zone = (await _fedExZoneRepository.Query(p => p.Id== id).SelectAsync(false)).FirstOrDefault();
            return zone;
        }

        public async Task<VitalGreenRequest> InsertRequest(VitalGreenRequest request)
        {
            request.DateView = request.DateCompleted = DateTime.Now;
            await _vitalGreenRequestRepository.InsertAsync(request);
            return request;
        }
    }
}
