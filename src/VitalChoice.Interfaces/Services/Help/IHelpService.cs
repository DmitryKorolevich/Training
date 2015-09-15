using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.Help;
using VitalChoice.Domain.Mail;
using VitalChoice.Domain.Transfer.Affiliates;
using VitalChoice.Domain.Transfer.Azure;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Help;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Help
{
    public interface IHelpService
    {
        #region HelpTicket

        Task<PagedList<VHelpTicket>> GetHelpTicketsAsync(VHelpTicketFilter filter);

        Task<HelpTicket> GetHelpTicketAsync(int id);

        Task<HelpTicket> UpdateHelpTicketAsync(HelpTicket model, int? adminId);

        Task<bool> DeleteHelpTicketAsync(int id);

        Task<HelpTicketComment> GetHelpTicketCommentAsync(int id);

        Task<HelpTicketComment> UpdateHelpTicketCommentAsync(HelpTicketComment model);

        Task<bool> DeleteHelpTicketCommentAsync(int id, int? adminId);

        #endregion

        #region BugTickets

        Task<PagedList<BugTicket>> GetBugTicketsAsync(VHelpTicketFilter filter);

        Task<BugTicket> GetBugTicketAsync(int id);

        Task<BugTicket> UpdateBugTicketAsync(BugTicket item, int adminId, bool? isSuperAdmin = null);

        Task<bool> DeleteBugTicketAsync(int id);

        Task<BugTicketComment> GetBugTicketCommentAsync(int id);

        Task<BugTicketComment> UpdateBugTicketCommentAsync(BugTicketComment item);

        Task<bool> DeleteBugTicketCommentAsync(int id, int adminId);

        Task<BugFile> AddBugFileAsync(BugFile file);

        Task<bool> DeleteBugFileAsync(int id);

        #endregion

        #region BugFiles

        Task<string> UploadBugFileToStoreAsync(BugFileType type, byte[] file, string fileName, string publicId, string contentType = null);

        Task<Blob> DownloadBugFileAsync(BugFileType type, string fileName, string publicId);

        Task<bool> DeleteBugFileFromStoreAsync(BugFileType type, string fileName, string publicId);

        #endregion
    }
}
