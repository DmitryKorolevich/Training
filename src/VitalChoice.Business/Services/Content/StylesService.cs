using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
	public class StylesService : GenericService<CustomPublicStyle>, IStylesService
    {
        private readonly IObjectLogItemExternalService _objectLogItemExternalService;

        public StylesService(IRepositoryAsync<CustomPublicStyle> repository,
            IObjectLogItemExternalService objectLogItemExternalService) : base(repository)
        {
            _objectLogItemExternalService = objectLogItemExternalService;
        }

		public async Task<CustomPublicStyle> GetStyles()
		{
			var obj = await Repository.Query().SelectFirstOrDefaultAsync(false);
			return obj;
		}

		public async Task UpdateStylesAsync(CustomPublicStyle customPublicStyle)
		{
			await UpdateAsync(customPublicStyle);
            customPublicStyle.StatusCode=RecordStatusCode.Active;
            await _objectLogItemExternalService.LogItem(customPublicStyle);
        }
	}
}
