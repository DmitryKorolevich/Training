﻿using System;
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
    [ApiValidator(typeof(ArticleManageModelValidator))]
    public class ArticleManageModel : Model<Article, IMode>
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

        public string SubTitle { get; set; }

        public string Author { get; set; }

        public DateTime? PublishedDate { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public IEnumerable<int> CategoryIds { get; set; }

        public ArticleManageModel()
        {
        }

        public ArticleManageModel(Article item)
        {
            Id = item.Id;
            Name = item.Name;
            Url = item.Url;
            FileUrl = item.FileUrl;
            SubTitle = item.SubTitle;
            Author = item.Author;
            PublishedDate = item.PublishedDate;
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
            if (item.ArticlesToContentCategories != null)
            {
                CategoryIds = item.ArticlesToContentCategories.Select(p => p.ContentCategoryId).ToList();
            }
        }

        public override Article Convert()
        {
            Article toReturn = new Article();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Url = Url?.Trim();
            toReturn.Url = toReturn.Url?.ToLower();
            toReturn.FileUrl = FileUrl?.Trim();
            toReturn.SubTitle = SubTitle;
            toReturn.Author = Author;
            toReturn.PublishedDate = PublishedDate;
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