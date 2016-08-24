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
        private readonly IObjectLogItemExternalService _objectLogItemExternalService;

        public ContentAreaService(
            IRepositoryAsync<ContentArea> repository,
            IObjectLogItemExternalService objectLogItemExternalService) : base(repository)
        {
            _objectLogItemExternalService = objectLogItemExternalService;
        }

		public async Task<ContentArea> GetContentAreaAsync(int id)
		{
			return await Query(id);
		}

		public async Task<ContentArea> GetContentAreaByNameAsync(string name)
		{
			return await Repository.Query(x => x.Name==name).SelectFirstOrDefaultAsync(false);
		}

        public async Task<ICollection<ContentArea>> GetContentAreaByNameAsync(ICollection<string> names)
        {
            return await Repository.Query(x => names.Contains(x.Name)).SelectAsync(false);
        }

        public async Task<IList<ContentArea>> GetContentAreasAsync()
		{
			return await Repository.Query().Include(x=>x.User).ThenInclude(x=>x.Profile).SelectAsync(false);
		}

		public async Task UpdateContentAreaAsync(ContentArea contentArea)
		{
			await UpdateAsync(contentArea);

            await _objectLogItemExternalService.LogItem(contentArea);
		}
	}
}
