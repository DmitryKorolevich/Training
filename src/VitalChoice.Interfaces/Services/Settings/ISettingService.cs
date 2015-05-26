using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Interfaces.Services.Settings
{
	public interface ISettingService
    {
        Task<AppSettingItem> GetAppSettingAsync(int id);

        Task<AppSettingItem> UpdateAppSettingAsync(int id, string value);

        Task<AppSettings> GetAppSettingsAsync();
    }
}
