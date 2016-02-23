using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Content.ContentCrossSells;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
	public class ContentCrossSellService: IContentCrossSellService
	{
		private readonly IRepositoryAsync<ContentCrossSell> _repository;
		private readonly IOptions<AppOptionsBase> _options;

		public ContentCrossSellService(IRepositoryAsync<ContentCrossSell> repository, IOptions<AppOptionsBase> options)
		{
			_repository = repository;
			_options = options;
		}

		public async Task<IList<ContentCrossSell>> GetContentCrossSells(ContentCrossSellType type)
		{
			var crossLines = await _repository.Query(x => x.Type == ContentCrossSellType.Default || x.Type == type).SelectAsync();

			var res = new List<ContentCrossSell>();
			for (var i = 0; i < ContentConstants.CONTENT_CROSS_SELL_LIMIT; i++)
			{
				var toAdd = crossLines.FirstOrDefault(x => x.Type == type && x.Order == i + 1) ??
				            crossLines.Single(x => x.Order == i + 1);

				res.Add(toAdd);
			}

			return res.OrderBy(x=>x.Order).ToList();
		}

		public async Task<IList<ContentCrossSell>> GetDefaultContentCrossSells()
		{
			var crossLines = await _repository.Query(x => x.Type == ContentCrossSellType.Default).SelectAsync();

			return crossLines;
		}

		public async Task<IList<ContentCrossSell>> UpdateContentCrossSells(IList<ContentCrossSell> contentCrossSells, ContentCrossSellType type)
		{
			var toWork = contentCrossSells.Where(x => x.Type == type).ToList();

			var toAdd = toWork.Where(x => x.Id != 0);
			var toUpdate = toWork.Where(x => x.Id == 0);

			using (var uow = new UnitOfWork(new VitalChoiceContext(_options)))
			{
				var uowRepo = uow.RepositoryAsync<ContentCrossSell>();

				using (var transaction = uow.BeginTransaction())
				{
					try
					{
						var updateRes = false;

						var addRes = await uowRepo.InsertRangeAsync(toAdd);
						if (addRes)
						{
							updateRes = await uowRepo.UpdateRangeAsync(toUpdate);
						}

						if (addRes && updateRes)
						{
							transaction.Commit();
						}
					}
					catch (Exception)
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
