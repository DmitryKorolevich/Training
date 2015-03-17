using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class MasterContentItem : Entity
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public ContentType Type { get; set; }                
    }
}