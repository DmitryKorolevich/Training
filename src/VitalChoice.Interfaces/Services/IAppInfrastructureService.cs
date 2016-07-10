using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services
{
    public interface IAppInfrastructureService
    {
        ReferenceData CachedData { get; }
        Task<ReferenceData> GetDataAsync();
    }
}