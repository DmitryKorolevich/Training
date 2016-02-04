using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.User;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
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

        public async Task<Token> GetTokenAsync(Guid id, TokenType idTokenType)
        {
            return (await _tokenRepository.Query(p => p.IdToken == id && p.IdTokenType== idTokenType).SelectAsync(false)).FirstOrDefault();
        }

        public async Task<Token> InsertTokenAsync(Token item)
        {
            await _tokenRepository.InsertAsync(item);
            return item;
        }
    }
}
