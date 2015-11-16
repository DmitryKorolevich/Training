using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VitalChoice.Interfaces.Services
{
    public interface IBackendSettingsService
    {
        Country GetDefaultCountry();
    }
}
