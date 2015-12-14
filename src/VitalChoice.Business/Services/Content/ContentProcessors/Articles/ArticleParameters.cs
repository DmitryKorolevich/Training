using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticleParameters
    {
        public int Page { get; set; }

        public bool Preview { get; set; }

        public string Url { get; set; }
    }
}
