using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticleParameters
    {
        public const string ArticleBaseUrl = "/article/";
        public const string ArticleCategoryBaseUrl = "/articles/";

        public bool BPreview { get; set; }

        public string Url { get; set; }
    }
}
