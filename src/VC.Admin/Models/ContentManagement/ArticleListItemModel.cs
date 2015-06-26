using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;

namespace VC.Admin.Models.ContentManagement
{
    public class ArticleListItemModel : Model<Article, IMode>
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

        public string Url { get; set; }

        public ICollection<string> Categories { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string SubTitle { get; set; }

        public string Author { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string AgentId { get; set; }

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
                    Categories = item.ArticlesToContentCategories.OrderBy(p => p.ContentCategory.Name).Select(p => p.ContentCategory.Name).ToArray();
                }
                if(item.ContentItem!=null)
                {
                    Created = item.ContentItem.Created;
                    Updated = item.ContentItem.Updated;
                }
                if (item.User != null && item.User.Profile != null)
                {
                    AgentId = item.User.Profile.AgentId;
                }
            }
        }
    }
}