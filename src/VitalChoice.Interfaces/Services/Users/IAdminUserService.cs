using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Interfaces.Services.Users
{
    public interface IAdminUserService: IUserService
    {
		Task<bool> IsSuperAdmin(ApplicationUser user);
	}
}