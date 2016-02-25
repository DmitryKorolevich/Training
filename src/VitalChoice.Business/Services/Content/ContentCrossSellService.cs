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
			var crossLines = await _repository.Query(x => x.Type == type).SelectAsync();

			return crossLines.OrderBy(x=>x.Order).ToList();
		}

		public async Task<IList<ContentCrossSell>> AddUpdateContentCrossSells(IList<ContentCrossSell> contentCrossSells)
		{
			var toAdd = contentCrossSells.Where(x => x.Id == 0).ToList();
			var toUpdate = contentCrossSells.Where(x => x.Id != 0).ToList();

			using (var uow = new UnitOfWork(new VitalChoiceContext(_options)))
			{
				var uowRepo = uow.RepositoryAsync<ContentCrossSell>();

				using (var transaction = uow.BeginTransaction())
				{
					try
					{
						var addRes = true;
						var updateRes = true;
						if (toAdd.Any())
						{
							addRes = await uowRepo.InsertRangeAsync(toAdd);
						}
						if (addRes && toUpdate.Any())
						{
							updateRes = await uowRepo.UpdateRangeAsync(toUpdate);
						}

						if (addRes && updateRes)
						{
							await uow.SaveChangesAsync();
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
