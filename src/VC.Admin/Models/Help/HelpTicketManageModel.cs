using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Interfaces;
using VC.Admin.Validators.Affiliate;
using VitalChoice.Domain.Entities.eCommerce.Help;

namespace VC.Admin.Models.Help
{
    [ApiValidator(typeof(HelpTicketManageModelValidator))]
    public class HelpTicketManageModel : BaseModel
    {
        public int Id { get; set; }

        public int IdOrder { get; set; }

        public int IdCustomer { get; set; }

        public string Customer { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public HelpTicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public ICollection<HelpTicketCommentManageModel> Comments { get; set; }

        public HelpTicketManageModel(HelpTicket item)
        {
            if (item != null)
            {
                Id = item.Id;
                IdOrder = item.IdOrder;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                StatusCode = item.StatusCode;
                Priority = item.Priority;
                Summary = item.Summary;
                Description = item.Description;
                IdCustomer = item.IdCustomer;
                Customer = item.Customer;
            }
        }

        public HelpTicket Convert()
        {
            HelpTicket toReturn = new HelpTicket();
            toReturn.Id = Id;
            toReturn.IdOrder = IdOrder;
            toReturn.StatusCode = StatusCode;
            toReturn.Priority = Priority;
            toReturn.Summary = Summary;
            toReturn.Description = Description;

            return toReturn;
        }
    }
}