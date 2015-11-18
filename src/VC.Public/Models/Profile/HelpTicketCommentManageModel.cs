using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Help;

namespace VC.Public.Models.Profile
{
    public class HelpTicketCommentManageModel : BaseModel
    {
        public int Id { get; set; }

        public int IdHelpTicket { get; set; }

        public int IdOrder { get; set; }

        public int IdCustomer { get; set; }

        public string Customer { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public string EditedBy { get; set; }

        public string Comment { get; set; }

        public HelpTicketCommentManageModel()
        {
        }

        public HelpTicketCommentManageModel(HelpTicketComment item)
        {
            if (item != null)
            {
                Id = item.Id;
                IdHelpTicket = item.IdHelpTicket;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                StatusCode = item.StatusCode;
                Comment = item.Comment;
                EditedBy = item.EditedBy;
                IdOrder = item.HelpTicket.IdOrder;
                IdCustomer = item.HelpTicket.IdCustomer;
                Customer = item.HelpTicket.Customer;
            }
        }

        public HelpTicketComment Convert()
        {
            HelpTicketComment toReturn = new HelpTicketComment();
            toReturn.Id = Id;
            toReturn.IdHelpTicket = IdHelpTicket;
            toReturn.Comment = Comment;

            return toReturn;
        }
    }
}