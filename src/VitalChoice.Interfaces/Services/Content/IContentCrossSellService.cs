using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Content.ContentCrossSells;

namespace VitalChoice.Interfaces.Services.Content
{
    public interface IContentCrossSellService
	{
	    Task<IList<ContentCrossSell>> GetContentCrossSells(ContentCrossSellType type);

	    Task<IList<ContentCrossSell>> AddUpdateContentCrossSells(IList<ContentCrossSell> contentCrossSells);
	}
}
