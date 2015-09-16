using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Transfer.Base;
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

		public async Task<CustomPublicStyle> UpdateStylesAsync(CustomPublicStyle customPublicStyle)
		{
			return await UpdateAsync(customPublicStyle);
		}
	}
}
