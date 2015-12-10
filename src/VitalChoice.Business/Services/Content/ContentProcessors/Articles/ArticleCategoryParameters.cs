﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticleCategoryParameters
    {
        public int? ArticlesPageIndex { get; set; }

        public bool BPreview { get; set; }

        public string Url { get; set; }
    }
}
