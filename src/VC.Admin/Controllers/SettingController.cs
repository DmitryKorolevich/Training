using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Models.Setting;
using VitalChoice.Validation.Controllers;
using VitalChoice.Validation.Models;

namespace VitalChoice.Controllers
{
    public class SettingController : BaseApiController
    {
        private readonly ILogViewService logViewService;
        private readonly ILogger logger;

        public SettingController(ILogViewService logViewService)
        {
            this.logViewService = logViewService;
            this.logger = LoggerService.GetDefault();
        }

        [HttpPost]
        public async Task<Result<PagedList<LogListItemModel>>> GetLogItems([FromBody]LogItemListFilter filter)
        {
            var result = await logViewService.GetCommonItemsAsync(filter.LogLevel,filter.Message, filter.Source, filter.From, filter.To?.AddDays(1),
                filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var toReturn = new PagedList<LogListItemModel>
            {
                Items = result.Items.Select(p=>new LogListItemModel(p)).ToList(),
                Count= result.Count,
            };

            return toReturn;
        }

        
    }
}