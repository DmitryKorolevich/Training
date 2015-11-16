using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Content;

namespace VitalChoice.Interfaces.Services.Content
{
    public interface IStylesService
	{
	    Task<CustomPublicStyle> GetStyles();

	    Task<CustomPublicStyle> UpdateStylesAsync(CustomPublicStyle customPublicStyle);
    }
}
