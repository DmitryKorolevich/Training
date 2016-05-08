using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Emails;
using VitalChoice.Infrastructure.Domain.Content.Recipes;


namespace VC.Admin.Models.EmailTemplates
{
    public class EmailTemplateListItemModel : BaseModel
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

        public string Subject { get; set; }

        public string EmailDescription { get; set; }
        
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string AgentId { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public EmailTemplateListItemModel(EmailTemplate item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Name = item.Name;
                EmailDescription = item.EmailDescription;
                StatusCode = item.StatusCode;
                if (item.ContentItem!=null)
                {
                    Created = item.ContentItem.Created;
                    Updated = item.ContentItem.Updated;
                    Subject = item.ContentItem.Title;
                }
                if (item.User != null && item.User.Profile != null)
                {
                    AgentId = item.User.Profile.AgentId;
                }
            }
        }
    }
}