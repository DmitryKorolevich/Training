using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.VitalGreen;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class VitalGreenService : IVitalGreenService
    {
        private readonly IRepositoryAsync<FedExZone> _fedExZoneRepository;
        private readonly IRepositoryAsync<VitalGreenRequest> _vitalGreenRequestRepository;
        private readonly IFedExService _fedExService;

        public VitalGreenService(
            IRepositoryAsync<FedExZone> fedExZoneRepository,
            IRepositoryAsync<VitalGreenRequest> vitalGreenRequestRepository,
            IFedExService fedExService)
        {
            _fedExZoneRepository = fedExZoneRepository;
            _vitalGreenRequestRepository = vitalGreenRequestRepository;
            _fedExService = fedExService;
        }
    }
}
