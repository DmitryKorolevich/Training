﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Content.ContentCrossSells;
using VitalChoice.Infrastructure.UOW;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Content
{
	public class ContentCrossSellService: IContentCrossSellService
	{
		private readonly IRepositoryAsync<ContentCrossSell> _repository;
		private readonly IOptions<AppOptionsBase> _options;
		private readonly IRepositoryAsync<Sku> _skuRepositoryAsync;
	    private readonly DbContextOptions<VitalChoiceContext> _contextOptions;
	    private readonly ILogger _logger;

	    public ContentCrossSellService(IRepositoryAsync<ContentCrossSell> repository, IOptions<AppOptionsBase> options,
	        IEcommerceRepositoryAsync<Sku> skuRepositoryAsync, ILoggerFactory loggerProvider, DbContextOptions<VitalChoiceContext> contextOptions)
	    {
	        _repository = repository;
	        _options = options;
	        _skuRepositoryAsync = skuRepositoryAsync;
	        _contextOptions = contextOptions;
	        _logger = loggerProvider.CreateLogger<ContentCrossSellService>();

	    }

	    public async Task<IList<ContentCrossSell>> GetContentCrossSellsAsync(ContentCrossSellType type)
	    {
	        var crossLines = await _repository.Query(x => x.Type == type).SelectAsync(false);

			return crossLines.OrderBy(x=>x.Order).ToList();
		}

		public async Task<IList<ContentCrossSell>> AddUpdateContentCrossSellsAsync(IList<ContentCrossSell> contentCrossSells)
		{
			if (contentCrossSells == null || !(contentCrossSells.Count > 0))
			{
				throw new ApiException("Can't be null or empty");
			}

			var ids = contentCrossSells.Select(x => x.IdSku).Distinct().ToList();

		    var notValidSku = await _skuRepositoryAsync.Query()
		        .Include(x => x.Product)
		        .Where(
		            x => ids.Contains(x.Id) &&
		                 (x.Hidden || x.StatusCode != (int) RecordStatusCode.Active || x.Product.IdVisibility == null ||
		                  x.Product.StatusCode != (int) RecordStatusCode.Active))
		        .SelectFirstOrDefaultAsync(false);
			if (notValidSku != null)
			{
				throw new AppValidationException($"Only active and not hidden SKUs are available. Replace {notValidSku.Code} with active and not hidden SKU");
			}

			var toAdd = contentCrossSells.Where(x => x.Id == 0).ToList();
			var toUpdate = contentCrossSells.Where(x => x.Id != 0).ToList();

			using (var uow = new VitalChoiceUnitOfWork(_contextOptions, _options))
			{
				var uowRepo = uow.RepositoryAsync<ContentCrossSell>();

				using (var transaction = uow.BeginTransaction())
				{
					try
					{
						var addRes = true;
						var updateRes = true;
						if (toAdd.Count > 0)
						{
							addRes = await uowRepo.InsertRangeAsync(toAdd);
						}
						if (addRes && toUpdate.Count > 0)
						{
							updateRes = await uowRepo.UpdateRangeAsync(toUpdate);
						}

						if (addRes && updateRes)
						{
							await uow.SaveChangesAsync();
							transaction.Commit();
						}
					}
                    catch
                    {
                        transaction.Rollback();
						throw;
					}
				}
			}

			return contentCrossSells;
		}
	}
}
