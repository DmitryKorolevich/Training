﻿using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Infrastructure.Domain.Transfer.ContentManagement
{
    public class CategoryTreeFilter : FilterBase
    {
	    public ContentType Type { get; set; }
        public RecordStatusCode? Status { get; set; }
    }
}