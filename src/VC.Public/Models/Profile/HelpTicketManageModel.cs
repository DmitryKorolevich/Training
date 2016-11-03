using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.DynamicData.Interfaces;
using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VC.Public.Models.Profile
{
    public class HelpTicketManageModel : BaseModel
    {
        [Display(Name = "Message #")]
        public int Id { get; set; }

        [Display(Name = "Order #")]
        public int IdOrder { get; set; }

        public int IdCustomer { get; set; }

        public string Customer { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        [Required]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Summary")]
        public string Summary { get; set; }

        [Required]
        [MaxLength(BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public ICollection<HelpTicketCommentManageModel> Comments { get; set; }

        public HelpTicketManageModel()
        {
        }

        public HelpTicketManageModel(HelpTicket item)
        {
            if (item != null)
            {
                Id = item.Id;
                IdOrder = item.IdOrder;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                Summary = item.Summary;
                Description = item.Description;
                IdCustomer = item.IdCustomer;
                Customer = item.Customer;
                if(item.Comments!=null)
                {
                    Comments = new List<HelpTicketCommentManageModel>();
                    foreach(var comment in item.Comments)
                    {
                        Comments.Add(new HelpTicketCommentManageModel(comment));
                    }
                }
            }
        }

        public HelpTicket Convert()
        {
            HelpTicket toReturn = new HelpTicket();
            toReturn.Id = Id;
            toReturn.IdOrder = IdOrder;
            toReturn.Priority= TicketPriority.Medium;
            toReturn.StatusCode = RecordStatusCode.Active;
            toReturn.Summary = Summary;
            toReturn.Description = Description;

            return toReturn;
        }
    }
}