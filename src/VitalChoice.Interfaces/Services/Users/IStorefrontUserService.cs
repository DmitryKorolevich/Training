using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Interfaces.Services.Users
{
    public interface IStorefrontUserService: IUserService
    {
	    Task SendSuccessfulRegistration(string email, string firstName, string lastName);

	    Task<string> GenerateLoginTokenAsync(Guid publicId);
    }
}