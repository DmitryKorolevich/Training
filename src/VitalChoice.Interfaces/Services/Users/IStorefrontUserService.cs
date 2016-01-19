using System;
using System.Threading.Tasks;

namespace VitalChoice.Interfaces.Services.Users
{
    public interface IStorefrontUserService: IUserService
    {
	    Task SendSuccessfulRegistration(string email, string firstName, string lastName);

        Task SendWholesaleSuccessfulRegistration(string email, string firstName, string lastName);

        Task<string> GenerateLoginTokenAsync(Guid publicId);
    }
}