using System;
using System.Linq;
using System.Collections.Generic;
using VC.Admin.Validators.ContentManagement;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;

namespace VitalChoice.Models.ContentManagement
{
    [ApiValidator(typeof(ContentPageManageModelValidator))]
    public class ContentPageManageModel : Model<ContentPage, IMode>
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.Title)]
        public string Name { get; set; }

        [Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        [Localized(GeneralFieldNames.Description)]
        public string Description { get; set; }

        public string FileUrl { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public int MasterContentItemId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public IEnumerable<int> CategoryIds { get; set; }

        public ContentPageManageModel()
        {
        }

        public ContentPageManageModel(ContentPage item)
        {
            Id = item.Id;
            Name = item.Name;
            Url = item.Url;
            FileUrl = item.FileUrl;
            StatusCode = item.StatusCode;
            Assigned = item.Assigned;
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
                ProcessorIds = item.ContentItem.ContentItemToContentProcessors.Select(p => p.ContentProcessorId).ToList();
            }
            else
            {
                ProcessorIds = new List<int>();
            }
            if (item.ContentPagesToContentCategories != null)
            {
                CategoryIds = item.ContentPagesToContentCategories.Select(p => p.ContentCategoryId).ToList();
            }
        }

        public override ContentPage Convert()
        {
            ContentPage toReturn = new ContentPage();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Url = Url?.Trim();
            toReturn.Url = toReturn.Url?.ToLower();
            toReturn.FileUrl = FileUrl?.Trim();
            toReturn.StatusCode = StatusCode;
            toReturn.Assigned = Assigned;
            toReturn.MasterContentItemId = MasterContentItemId;
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
                    ContentProcessorId = p,
                }).ToList();
            }

            return toReturn;
        }
    }
}