using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;

namespace VitalChoice.Interfaces.Services
{
	public interface ITokenService
    {
        Task<Token> GetTokenAsync(Guid id, TokenType idTokenType);

        Task<Token> InsertTokenAsync(Token item);
    }
}
