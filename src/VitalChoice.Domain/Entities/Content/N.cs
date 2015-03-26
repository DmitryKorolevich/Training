using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class N : Entity
    {
        public string Name { get; set; }

        public ContentItemProcessor ContentItemProcessor { get; set; }
    }
}