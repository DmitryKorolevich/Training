using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
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

        public bool HideLongDescription { get; set; }

        public string LongDescriptionBottom { get; set; }

        public bool HideLongDescriptionBottom { get; set; }

        public string FileImageSmallUrl { get; set; }

        public string FileImageLargeUrl { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int MasterContentItemId { get; set; }

        public int? ParentId { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public CustomerTypeCode Assigned { get; set; }

        public string NavLabel { get; set; }

        public CustomerTypeCode? NavIdVisible { get; set; }

        public ProductCategoryViewType ViewType { get; set; }

        public ProductCategoryManageModel()
        {
        }

        public ProductCategoryManageModel(ProductCategoryContent item)
        {
            Id = item.Id;
            Name = item.ProductCategory.Name;
            Url = item.Url;
            FileImageSmallUrl = item.FileImageSmallUrl;
            FileImageLargeUrl = item.FileImageLargeUrl;
            StatusCode = item.StatusCode;
            Assigned = item.ProductCategory.Assigned;
            ParentId = item.ProductCategory.ParentId;
            MasterContentItemId = item.MasterContentItemId;
            LongDescription = item.LongDescription;
            HideLongDescription = item.HideLongDescription;
            LongDescriptionBottom = item.LongDescriptionBottom;
            HideLongDescriptionBottom = item.HideLongDescriptionBottom;
            NavLabel = item.NavLabel;
            NavIdVisible = item.NavIdVisible;
            ViewType = item.ViewType;
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
                Url = Url?.Trim().ToLower(),
                FileImageSmallUrl = FileImageSmallUrl?.Trim(),
                FileImageLargeUrl = FileImageLargeUrl?.Trim(),
                StatusCode = StatusCode,
                ProductCategory = {
                    Name = Name?.Trim(),
                    Assigned = Assigned,
                    ParentId = ParentId,
                    StatusCode = StatusCode,
                },
                MasterContentItemId = MasterContentItemId,
                LongDescription = LongDescription,
                HideLongDescription = HideLongDescription,
                LongDescriptionBottom = LongDescriptionBottom,
                HideLongDescriptionBottom = HideLongDescriptionBottom,
                NavLabel = NavLabel,
                NavIdVisible = NavIdVisible,
                ViewType = ViewType,
                ContentItem = new ContentItem
                {
                    Description = Description ?? string.Empty,
                    Template = Template,
                    Title = Title,
                    MetaKeywords = MetaKeywords,
                    MetaDescription = MetaDescription
                }
            };
            
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