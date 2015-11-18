﻿using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Transfer.Help;

namespace VitalChoice.SharedWeb.Models.Help
{
    public class HelpTicketListItemModel : BaseModel
    {
        public int Id { get; set; }

        public int IdOrder { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public TicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public int IdCustomer { get; set; }

        public string Customer { get; set; }

        public HelpTicketListItemModel(VHelpTicket item)
        {
            if(item!=null)
            {
                Id = item.Id;
                IdOrder = item.IdOrder;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                StatusCode = item.StatusCode;
                Priority = item.Priority;
                Summary = item.Summary;
                IdCustomer = item.IdCustomer;
                Customer = item.Customer;
            }
        }
    }
}