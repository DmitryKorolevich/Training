using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators;
using VC.Admin.Validators.ContentManagement;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Emails;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.EmailTemplates
{
    [ApiValidator(typeof(EmailTemplateManageModelValidator))]
    public class EmailTemplateManageModel : BaseModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string EmailDescription { get; set; }

        public string Template { get; set; }

        public string Subject { get; set; }

        public ICollection<string> ModelPropertyNames { get; set; }

        public int MasterContentItemId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public EmailTemplateManageModel()
        { }

        public EmailTemplateManageModel(EmailTemplate item)
        {
            Id = item.Id;
            Name = item.Name;
            EmailDescription = item.EmailDescription;
            StatusCode = item.StatusCode;
            Template = item.ContentItem.Template;
            Subject = item.ContentItem.Title;
            ModelPropertyNames = item.ModelPropertyNames;
            Created = item.ContentItem.Created;
            Updated = item.ContentItem.Updated;
            MasterContentItemId = item.MasterContentItemId;
		}

        public EmailTemplate Convert()
        {
            var toReturn = new EmailTemplate();
            toReturn.Id = Id;
            toReturn.ContentItem = new ContentItem();
            toReturn.ContentItem.Template = Template;
            toReturn.ContentItem.Description =String.Empty;
            toReturn.ContentItem.Title = Subject;
            toReturn.MasterContentItemId = MasterContentItemId;

            return toReturn;
        }
    }
}