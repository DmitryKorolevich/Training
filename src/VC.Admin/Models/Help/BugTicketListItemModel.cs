using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.Help;

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
            }
        }
    }
}