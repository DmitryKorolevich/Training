using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Transfer.Azure;
using VitalChoice.Infrastructure.Domain.Transfer.Help;

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

        Task<bool> DeleteBugTicketAsync(int id, int adminId, int? userId=null);

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
