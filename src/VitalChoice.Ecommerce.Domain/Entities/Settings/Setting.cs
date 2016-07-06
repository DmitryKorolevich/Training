using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Ecommerce.Domain.Entities.Settings
{
    public class Setting : DynamicDataEntity<SettingOptionValue, SettingOptionType>
	{
	    public Setting()
	    {
        }
    }
}
