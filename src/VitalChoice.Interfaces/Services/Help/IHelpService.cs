using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Mail;
using VitalChoice.Domain.Transfer.Affiliates;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Help;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Help
{
    public interface IHelpService
    {
        Task<PagedList<VHelpTicket>> GetHelpTicketsAsync(VHelpTicketFilter filter);

        Task<HelpTicket> GetHelpTicketAsync(int id);

        Task<HelpTicket> UpdateHelpTicketAsync(HelpTicket model, int? adminId);

        Task<bool> DeleteHelpTicketAsync(int id);

        Task<HelpTicketComment> GetHelpTicketCommentAsync(int id);

        Task<HelpTicketComment> UpdateHelpTicketCommentAsync(HelpTicketComment model);

        Task<bool> DeleteHelpTicketCommentAsync(int id, int? adminId);
    }
}
