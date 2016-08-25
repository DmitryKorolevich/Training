using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;

namespace VitalChoice.Interfaces.Services
{
    public interface ITokenService
    {
        Task<Token> GetValidToken(Guid id, TokenType type);
        Task ExpireToken(Guid id);
        Task<Token> CreateTokenAsync(string data, TimeSpan expiration, TokenType type);
    }
}