﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IProductViewService
    {
        Task<ExecutedContentItem> GetProductCategoryContentAsync(Dictionary<string, object> parameters, string categoryUrl = null);
    }
}