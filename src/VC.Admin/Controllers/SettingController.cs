using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Controllers;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Validation.Helpers.GlobalFilters;
using VitalChoice.Validation.Models;
using VitalChoice.Validators.Users;
using VitalChoice.Validation.Logic;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Core;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Business.Services.Contracts;
using Microsoft.Framework.Logging;
using System.Threading.Tasks;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Models.Setting;

namespace VitalChoice.Admin.Controllers
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