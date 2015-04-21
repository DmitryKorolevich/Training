using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services.Contracts
{
    public interface IUserService
    {
	    Task<ApplicationUser> CreateAsync(ApplicationUser user, IList<RoleType> roles);

	    Task DeleteAsync(ApplicationUser user);

	    Task<ApplicationUser> UpdateAsync(ApplicationUser user, IList<RoleType> roleIds = null);

	    Task<ApplicationUser> GetAsync(Guid publicId);

	    Task<PagedList<ApplicationUser>> GetAsync(FilterBase filter);
    }
}