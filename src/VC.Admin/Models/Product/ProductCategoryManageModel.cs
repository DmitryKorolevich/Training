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

        public string NavLabel { get; set; }

        public CustomerTypeCode? NavIdVisible { get; set; }

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
            Assigned = item.ProductCategory.Assigned;
            ParentId = item.ProductCategory.ParentId;
            MasterContentItemId = item.MasterContentItemId;
            LongDescription = item.LongDescription;
            LongDescriptionBottom = item.LongDescriptionBottom;
            NavLabel = item.NavLabel;
            NavIdVisible = item.NavIdVisible;
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
            ProductCategoryContent toReturn = new ProductCategoryContent
            {
                Id = Id,
                Name = Name?.Trim(),
                Url = Url?.Trim().ToLower(),
                FileImageSmallUrl = FileImageSmallUrl?.Trim(),
                FileImageLargeUrl = FileImageLargeUrl?.Trim(),
                StatusCode = StatusCode,
                ProductCategory = {Assigned = Assigned},
                MasterContentItemId = MasterContentItemId ?? 0,
                LongDescription = LongDescription,
                LongDescriptionBottom = LongDescriptionBottom,
                NavLabel = NavLabel,
                NavIdVisible = NavIdVisible,
                ContentItem = new ContentItem
                {
                    Description = Description ?? string.Empty,
                    Template = Template,
                    Title = Title,
                    MetaKeywords = MetaKeywords,
                    MetaDescription = MetaDescription
                }
            };
            toReturn.ProductCategory.ParentId = ParentId;
            
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