﻿using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;

namespace VitalChoice.Models.ContentManagement
{
    public class ArticleListItemModel : Model<Article, IMode>
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

        public string Url { get; set; }

        public string Category { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string SubTitle { get; set; }

        public string Author { get; set; }

        public DateTime? PublishedDate { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public ArticleListItemModel(Article item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Name = item.Name;
                Url = item.Url;
                StatusCode = item.StatusCode;
                SubTitle = item.SubTitle;
                Author = item.Author;
                PublishedDate = item.PublishedDate;
                if (item.ArticlesToContentCategories != null)
                {
                    foreach(var RecipesToContentCategory in item.ArticlesToContentCategories.OrderBy(p=>p.ContentCategory.Name))
                    {
                        if(RecipesToContentCategory.ContentCategory!=null)
                        {
                            Category += RecipesToContentCategory.ContentCategory.Name + ", ";
                        }
                    }
                }
                if(String.IsNullOrEmpty(Category))
                {
                    Category = ContentConstants.NO_CATEGORIES_LABEL;
                }
                else
                {
                    Category=Category.Remove(Category.Length - 2, 2);
                }
                if(item.ContentItem!=null)
                {
                    Created = item.ContentItem.Created;
                    Updated = item.ContentItem.Updated;
                }
            }
        }
    }
}