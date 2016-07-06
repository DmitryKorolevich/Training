using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VitalChoice.Interfaces.Services.Settings
{
    public interface ISettingService
    {
        Task<SettingDynamic> GetSettingsAsync();

        Task<bool> UpdateSettingsAsync(SettingDynamic settings);
        
        Task<IList<Lookup>> GetLookupsAsync(ICollection<string> names);

        Task<Lookup> GetLookupAsync(int id);

        Task<bool> UpdateLookupVariantsAsync(int id, ICollection<LookupVariant> variants);
    }
}