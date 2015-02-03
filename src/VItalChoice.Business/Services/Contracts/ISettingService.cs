using System;
using System.Collections.Generic;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Localization;

namespace VitalChoice.Business.Services.Contracts
{
	public interface ISettingService
	{
	    string GetProjectConstant(string key);
	}
}
