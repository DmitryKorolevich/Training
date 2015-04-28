using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Domain.Transfer.Product;

namespace VitalChoice.Business.Services.Contracts.Settings
{
	public interface ISettingService
    {
        Task<AppSettingItem> GetAppSettingAsync(int id);

        Task<AppSettingItem> UpdateAppSettingAsync(int id, string value);

        Task<AppSettings> GetAppSettingsAsync();
    }
}
