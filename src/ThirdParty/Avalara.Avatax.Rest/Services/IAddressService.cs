using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Avatax;

namespace Avalara.Avatax.Rest.Services
{
    public interface IAddressService
    {
        Task<ValidateResult> Validate(Address address);
    }
}