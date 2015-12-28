using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VC.Admin.Models.Help
{
    public class BugTicketListItemModel : BaseModel
    {
        public int Id { get; set; }

        public int IdOrder { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public TicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public int IdAddedBy { get; set; }

        public string AddedBy { get; set; }

        public string AddedByAgent { get; set; }

        public bool AllowDelete { get; set; }

        public int IdEditedBy { get; set; }

        public string EditedByAgent { get; set; }

        public BugTicketListItemModel(BugTicket item)
        {
            if(item!=null)
            {
                Id = item.Id;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                StatusCode = item.StatusCode;
                Priority = item.Priority;
                Summary = item.Summary;
                IdAddedBy = item.IdAddedBy;
                AddedBy = item.AddedBy;
                AddedByAgent = item.AddedByAgent;
                IdEditedBy = item.IdEditedBy;
                EditedByAgent = item.EditedByAgent;
            }
        }
    }
}