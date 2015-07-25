using System;
using System.Linq;
using System.Collections.Generic;
using VC.Admin.Validators.ContentManagement;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.ContentManagement
{
    [ApiValidator(typeof(FaqManageModelValidator))]
    public class FAQManageModel : BaseModel
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.Title)]
        public string Name { get; set; }

        [Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        [Localized(GeneralFieldNames.Description)]
        public string Description { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public int MasterContentItemId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public IEnumerable<int> CategoryIds { get; set; }

        public FAQManageModel()
        {
        }

        public FAQManageModel(FAQ item)
        {
            Id = item.Id;
            Name = item.Name;
            Url = item.Url;
            StatusCode = item.StatusCode;
            Template = item.ContentItem.Template;
            Description = item.ContentItem.Description;
            Title = item.ContentItem.Title;
            MetaKeywords = item.ContentItem.MetaKeywords;
            MetaDescription = item.ContentItem.MetaDescription;
            Created = item.ContentItem.Created;
            Updated = item.ContentItem.Updated;
            MasterContentItemId = item.MasterContentItemId;
            if (item.ContentItem.ContentItemToContentProcessors != null)
            {
                ProcessorIds = item.ContentItem.ContentItemToContentProcessors.Select(p => p.ContentItemProcessorId).ToList();
            }
            else
            {
                ProcessorIds = new List<int>();
            }
            if (item.FAQsToContentCategories != null)
            {
                CategoryIds = item.FAQsToContentCategories.Select(p => p.ContentCategoryId).ToList();
            }
        }

        public FAQ Convert()
        {
            FAQ toReturn = new FAQ();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Url = Url?.Trim();
            toReturn.Url = toReturn.Url?.ToLower();
            toReturn.FileUrl = String.Empty;
            toReturn.ContentItem = new ContentItem();
            toReturn.ContentItem.Template = Template;
            toReturn.ContentItem.Description = Description?.Trim();
            toReturn.ContentItem.Title = Title;
            toReturn.ContentItem.MetaKeywords = MetaKeywords;
            toReturn.ContentItem.MetaDescription = MetaDescription;
            if (ProcessorIds != null)
            {
                toReturn.ContentItem.ContentItemToContentProcessors = ProcessorIds.Select(p => new ContentItemToContentProcessor()
                {
                    ContentItemProcessorId = p,
                }).ToList();
            }

            return toReturn;
        }
    }
}