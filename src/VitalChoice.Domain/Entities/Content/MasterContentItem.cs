﻿using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class MasterContentItem : Entity
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public ContentType Type { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public virtual ICollection<ContentCategory> ContentCategories { get;set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}