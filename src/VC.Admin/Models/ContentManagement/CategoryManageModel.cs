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
    [ApiValidator(typeof(CategoryManageModelValidator))]
    public class CategoryManageModel : Model<ContentCategory, IMode>
    {
        public int Id { get; set; }
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        [Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        public string FileUrl { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public ContentType Type { get; set; }

        public int? MasterContentItemId { get; set; }

        public int? ParentId { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public CategoryManageModel()
        {
        }

        public CategoryManageModel(ContentCategory item)
        {
            Id = item.Id;
            Name = item.Name;
            Url = item.Url;
            FileUrl = item.FileUrl;
            StatusCode = item.StatusCode;
            ParentId = item.ParentId;
            MasterContentItemId = item.MasterContentItemId;
            Type = item.Type;
            Template = item.ContentItem.Template;
            Title = item.ContentItem.Title;
            MetaKeywords = item.ContentItem.MetaKeywords;
            MetaDescription = item.ContentItem.MetaDescription;
            Created = item.ContentItem.Created;
            Updated = item.ContentItem.Updated;
            if (item.ContentItem.ContentItemToContentProcessors != null)
            {
                ProcessorIds = item.ContentItem.ContentItemToContentProcessors.Select(p => p.ContentProcessorId).ToList();
            }
            else
            {
                ProcessorIds = new List<int>();
            }
        }

        public override ContentCategory Convert()
        {
            ContentCategory toReturn = new ContentCategory();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Url = Url?.Trim();
            toReturn.Url = toReturn.Url?.ToLower();
            toReturn.FileUrl = FileUrl?.Trim();
            toReturn.MasterContentItemId = MasterContentItemId.HasValue ? MasterContentItemId.Value : 0;
            toReturn.ParentId = ParentId;
            toReturn.Type = Type;
            toReturn.ContentItem = new ContentItem();
            toReturn.ContentItem.Template = Template;
            toReturn.ContentItem.Description = String.Empty;
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