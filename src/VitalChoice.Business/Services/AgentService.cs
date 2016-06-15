using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class AgentService: IAgentService
    {
        private readonly IReadRepositoryAsync<AdminProfile> _adminProfileRepositoryAsync;

        public AgentService(IReadRepositoryAsync<AdminProfile> adminProfileRepositoryAsync)
        {
            _adminProfileRepositoryAsync = adminProfileRepositoryAsync;
        }

        public async Task<Dictionary<int, string>> GetAgents(ICollection<int> ids = null)
        {
            if (ids != null)
                return (await _adminProfileRepositoryAsync.Query(a => ids.Contains(a.Id)).SelectAsync(false)).ToDictionary(a => a.Id,
                    a => a.AgentId);
            return (await _adminProfileRepositoryAsync.Query().SelectAsync(false)).ToDictionary(a => a.Id,
                a => a.AgentId);
        }
    }
}
