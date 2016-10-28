using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Transfer.Groove;

namespace VitalChoice.Interfaces.Services
{
    public interface IGrooveService
    {
        Task<bool> AddHelpTicketAsync(AddTicketModel ticket);
    }
}