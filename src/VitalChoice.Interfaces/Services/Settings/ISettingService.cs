using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Interfaces.Services.Settings
{
	public interface ISettingService
    {
        Task<ICollection<AppSettingItem>> GetAppSettingItemsAsync(ICollection<string> names);

        Task<ICollection<AppSettingItem>> UpdateAppSettingItemsAsync(ICollection<AppSettingItem> items);

        Task<AppSettings> GetAppSettingsAsync();
    }
}
