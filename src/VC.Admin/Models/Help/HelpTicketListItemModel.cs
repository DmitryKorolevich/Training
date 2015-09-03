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

namespace VC.Admin.Models.Help
{
    public class HelpTicketListItemModel : BaseModel
    {
        public int Id { get; set; }

        public int IdOrder { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public HelpTicketPriority Priority { get; set; }

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