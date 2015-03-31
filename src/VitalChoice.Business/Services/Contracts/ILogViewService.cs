using System;
using System.Collections.Generic;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Base;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Services.Contracts
{
	public interface ILogViewService
    {
        PagedList<CommonLogItem> GetCommonItems(string logLevel=null,string message=null, DateTime? from=null,DateTime? to=null,
            int page=1,int take = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT);
	}
}
