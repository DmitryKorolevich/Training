﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Infrastructure.UnitOfWork;
using System.Threading;
using Microsoft.Framework.ConfigurationModel;
using VitalChoice.Domain.Entities.Base;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Business.Queries.Log;
using VitalChoice.Domain.Constants;
using System.Threading.Tasks;

namespace VitalChoice.Business.Services.Impl
{
    public class LogViewService : ILogViewService
    {
        private readonly ILogsRepositoryAsync<CommonLogItem> commonLogItemsRepository;

        public LogViewService(ILogsRepositoryAsync<CommonLogItem> commonLogItemsRepository)
        {
            this.commonLogItemsRepository = commonLogItemsRepository;
        }

        public async Task<PagedList<CommonLogItem>> GetCommonItemsAsync(string logLevel = null, string message = null, string source = null, DateTime? from = null, DateTime? to = null,
            int page = 1, int take = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT)
        {
            var query = new CommonLogQuery();
            query = query.GetItems(logLevel, message, source, from, to);

            var toRetirn = await commonLogItemsRepository.Query(query).OrderBy(x=>x.OrderByDescending(pp=>pp.Date))
                .SelectPageAsync(page, take);

            return toRetirn;
        }
    }
}
