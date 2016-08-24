using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Entities.Tokens
{
    public enum TokenType
    {
        OrderInvoicePdfAdminGenerateRequest = 1,
        CustomerAutoReLoginToken = 2
    }
}