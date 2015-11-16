﻿using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Content.Base
{
    public class ContentItem : Entity
    {
        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string Template { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public ICollection<ContentItemToContentProcessor> ContentItemToContentProcessors { get; set; }
    }
}