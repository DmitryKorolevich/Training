using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services
{
	public interface IRedirectService
	{
        Task<PagedList<Redirect>> GetRedirectsAsync(FilterBase filter);

        Task<Redirect> GetRedirectAsync(int id);

        Task<Redirect> UpdateRedirectAsync(Redirect item);

        Task<bool> DeleteRedirectAsync(int id, int idUser);
    }
}
