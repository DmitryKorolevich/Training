using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Tokens
{
    public class Token : Entity
    {
        public Guid IdToken { get; set; }

        public DateTime DateCreated {get;set; }

        public DateTime DateExpired { get; set; }

        public TokenType IdTokenType { get; set; }

        public string Data { get; set; }
    }
}
