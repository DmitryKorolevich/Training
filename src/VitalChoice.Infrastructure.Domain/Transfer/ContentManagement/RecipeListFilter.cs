﻿using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.ContentManagement
{
    public class RecipeListFilter : FilterBase
    {
	    public string Name { get; set; }

        public int? CategoryId { get; set; }

	    public int? ProductId { get; set; }

        public RecordStatusCode? StatusCode { get; set; }
    }
}