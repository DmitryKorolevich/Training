using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Entities.Help
{
    public enum BugTicketStatus
    {
        NotActive = 1,
        Active = 2,
        Deleted = 3,
        Review = 4,
    }
}