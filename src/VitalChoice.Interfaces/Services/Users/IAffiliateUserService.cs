using System.Threading.Tasks;

namespace VitalChoice.Interfaces.Services.Users
{
    public interface IAffiliateUserService : IUserService
    {
	    Task SendSuccessfulRegistration(string email, string firstName, string lastName);

        Task<string> GenerateLoginTokenAsync(int id);
    }
}