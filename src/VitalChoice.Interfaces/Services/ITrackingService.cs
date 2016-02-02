using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VitalChoice.Interfaces.Services
{
    public interface ITrackingService
    {
        string GetServiceUrl(string carrier, string trackingNumber);
    }
}
