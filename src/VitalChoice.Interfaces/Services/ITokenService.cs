using Microsoft.AspNet.Http;
using System;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services
{
	public interface ITokenService
    {
        Task<Token> GetTokenAsync(Guid id, TokenType idTokenType);

        Task<Token> InsertTokenAsync(Token item);
    }
}
