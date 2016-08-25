using System;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class TokenService : ITokenService
    {
        private readonly IRepositoryAsync<Token> _tokenRepository;

        public TokenService(IRepositoryAsync<Token> tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<Token> GetValidToken(Guid id, TokenType type)
        {
            var token = await _tokenRepository.Query(p => p.IdToken == id).SelectFirstOrDefaultAsync(false);
            return token?.IdTokenType == type && token.DateExpired > DateTime.Now ? token : null;
        }

        public async Task ExpireToken(Guid id)
        {
            var token = await _tokenRepository.Query(t => t.IdToken == id).SelectFirstOrDefaultAsync(true);
            if (token != null)
            {
                token.DateExpired = DateTime.Now.AddDays(-1);
                await _tokenRepository.SaveChangesAsync();
            }
        }

        public async Task<Token> CreateTokenAsync(string data, TimeSpan expiration, TokenType type)
        {
            var now = DateTime.Now;
            var newToken = new Token
            {
                DateCreated = now,
                IdTokenType = type,
                DateExpired = now.AddTicks(expiration.Ticks),
                Data = data
            };
            if (await _tokenRepository.InsertAsync(newToken))
            {
                return newToken;
            }
            return null;
        }
    }
}