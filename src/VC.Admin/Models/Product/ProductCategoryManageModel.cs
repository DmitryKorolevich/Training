using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(ProductCategoryManageModelValidator))]
    public class ProductCategoryManageModel : BaseModel
    {
        public int Id { get; set; }
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        [Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        public string Description { get; set; }

        public string LongDescription { get; set; }

        public string LongDescriptionBottom { get; set; }

        public string FileImageSmallUrl { get; set; }

        public string FileImageLargeUrl { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int? MasterContentItemId { get; set; }

        public int? ParentId { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public ProductCategoryManageModel()
        {
        }

        public ProductCategoryManageModel(ProductCategoryContent item)
        {
            Id = item.Id;
            Name = item.Name;
            Url = item.Url;
            FileImageSmallUrl = item.FileImageSmallUrl;
            FileImageLargeUrl = item.FileImageLargeUrl;
            StatusCode = item.StatusCode;
            Assigned = item.Assigned;
            ParentId = item.ParentId;
            MasterContentItemId = item.MasterContentItemId;
            LongDescription = item.LongDescription;
            LongDescriptionBottom = item.LongDescriptionBottom;
            Description = item.ContentItem.Description;
            Template = item.ContentItem.Template;
            Title = item.ContentItem.Title;
            MetaKeywords = item.ContentItem.MetaKeywords;
            MetaDescription = item.ContentItem.MetaDescription;
            Created = item.ContentItem.Created;
            Updated = item.ContentItem.Updated;
            if (item.ContentItem.ContentItemToContentProcessors != null)
            {
                ProcessorIds = item.ContentItem.ContentItemToContentProcessors.Select(p => p.ContentItemProcessorId).ToList();
            }
            else
            {
                ProcessorIds = new List<int>();
            }
        }

        public ProductCategoryContent Convert()
        {
            ProductCategoryContent toReturn = new ProductCategoryContent();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Url = Url?.Trim();
            toReturn.Url = toReturn.Url?.ToLower();
            toReturn.FileImageSmallUrl = FileImageSmallUrl?.Trim();
            toReturn.FileImageLargeUrl = FileImageLargeUrl?.Trim();
            toReturn.StatusCode = StatusCode;
            toReturn.Assigned = Assigned;
            toReturn.MasterContentItemId = MasterContentItemId.HasValue ? MasterContentItemId.Value : 0;
            toReturn.ParentId = ParentId;
            toReturn.LongDescription = LongDescription;
            toReturn.LongDescriptionBottom = LongDescriptionBottom;
            toReturn.ContentItem = new ContentItem();
            toReturn.ContentItem.Description = Description;
            if(toReturn.ContentItem.Description==null)
            {
                toReturn.ContentItem.Description = String.Empty;
            }
            toReturn.ContentItem.Template = Template;
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