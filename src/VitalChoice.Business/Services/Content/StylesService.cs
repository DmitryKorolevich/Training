using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
	public class StylesService : GenericService<CustomPublicStyle>, IStylesService
	{
		public StylesService(IRepositoryAsync<CustomPublicStyle> repository) : base(repository)
		{
		}

		public async Task<CustomPublicStyle> GetStyles()
		{
			var obj = await Repository.Query().SelectFirstOrDefaultAsync(false);
			return obj;
		}

		public Task UpdateStylesAsync(CustomPublicStyle customPublicStyle)
		{
			return UpdateAsync(customPublicStyle);
		}
	}
}
