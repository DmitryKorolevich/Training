using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VitalChoice.Interfaces.Services.Settings
{
    public interface ISettingService
    {
        Task<List<AppSettingItem>> GetAppSettingItemsAsync(ICollection<string> names);

        Task<AppSettingItem> GetAppSettingAsync(string name);

        Task<List<AppSettingItem>> UpdateAppSettingItemsAsync(ICollection<AppSettingItem> items);

        Task<AppSettings> GetAppSettingsAsync();

        Task<IList<Lookup>> GetLookupsAsync(ICollection<string> names);

        Task<Lookup> GetLookupAsync(int id);

        Task<bool> UpdateLookupVariantsAsync(int id, ICollection<LookupVariant> variants);
    }
}