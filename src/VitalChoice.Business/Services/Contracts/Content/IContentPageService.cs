﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services.Contracts.Content
{
	public interface IContentPageService
    {
        Task<PagedList<ContentPage>> GetContentPagesAsync(string name=null,int? categoryId =null, int page = 1, int take = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT);
        Task<ContentPage> GetContentPageAsync(int id);
        Task<ContentPage> UpdateContentPageAsync(ContentPage contentPage);
        Task<bool> AttachContentPageToCategoriesAsync(int id, IEnumerable<int> categoryIds);
        Task<bool> DeleteContentPageAsync(int id);
    }
}
