using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Interfaces.Services
{
    public interface IUspsService
    {
        //only USA
        Task<bool> IsAddressValidAsync(AddressDynamic address);
    }
}