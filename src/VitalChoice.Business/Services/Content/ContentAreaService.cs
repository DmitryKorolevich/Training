using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
	public class ContentAreaService : GenericService<ContentArea>, IContentAreaService
	{
		public ContentAreaService(IRepositoryAsync<ContentArea> repository) : base(repository)
		{
		}

		public async Task<ContentArea> GetContentAreaAsync(int id)
		{
			return await Query(id);
		}

		public async Task<ContentArea> GetContentAreaByNameAsync(string name)
		{
			return await Repository.Query(x => x.Name.Equals(name)).SelectFirstOrDefaultAsync(false);
		}

		public async Task<IList<ContentArea>> GetContentAreasAsync()
		{
			return await Repository.Query().Include(x=>x.User).ThenInclude(x=>x.Profile).SelectAsync();
		}

		public async Task<ContentArea> UpdateContentAreaAsync(ContentArea contentArea)
		{
			return await UpdateAsync(contentArea);
		}
	}
}
