using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VitalChoice.Interfaces.Services.Settings
{
	public interface ISettingService
    {
        Task<List<AppSettingItem>> GetAppSettingItemsAsync(ICollection<string> names);

        Task<AppSettingItem> GetAppSettingAsync(string name);

        Task<List<AppSettingItem>> UpdateAppSettingItemsAsync(ICollection<AppSettingItem> items);

        Task<AppSettings> GetAppSettingsAsync();
    }
}
