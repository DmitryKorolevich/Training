﻿using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.ContentManagement;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.ContentManagement
{
    [ApiValidator(typeof(RecipeManageModelValidator))]
    public class RecipeManageModel : BaseModel
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

        public IEnumerable<int> ProcessorIds { get; set; }

        public IEnumerable<int> CategoryIds { get; set; }

        public IList<RecipeToProduct> RecipesToProducts { get; set; }

        public RecipeManageModel()
        {
        }

        public RecipeManageModel(Recipe item)
        {
            Id = item.Id;
            Name = item.Name;
            Url = item.Url;
            FileUrl = item.FileUrl;
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
                ProcessorIds = item.ContentItem.ContentItemToContentProcessors.Select(p => p.ContentProcessorId).ToList();
            }
            else
            {
                ProcessorIds = new List<int>();
            }
            if (item.RecipesToContentCategories != null)
            {
                CategoryIds = item.RecipesToContentCategories.Select(p => p.ContentCategoryId).ToList();
            }

            RecipesToProducts = item.RecipesToProducts.ToList();
        }

        public Recipe Convert()
        {
            Recipe toReturn = new Recipe();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Url = Url?.Trim();
            toReturn.Url = toReturn.Url?.ToLower();
            toReturn.FileUrl = FileUrl?.Trim();
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
            toReturn.RecipesToProducts = RecipesToProducts;

            return toReturn;
        }
    }
}