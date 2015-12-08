using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticleCategoryParameters
    {
        public const string ArticleBaseUrl = "/article/";
        public const string ArticleCategoryBaseUrl = "/articles/";

        public int? IdCategory { get; set; }

        public int? ArticlesPageIndex { get; set; }

        public bool Preview { get; set; }
    }
}
