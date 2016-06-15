using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Interfaces.Services
{
    public interface IAgentService
    {
        Task<Dictionary<int, string>> GetAgents(ICollection<int> ids = null);
    }
}