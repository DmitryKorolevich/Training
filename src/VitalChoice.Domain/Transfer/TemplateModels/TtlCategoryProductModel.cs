﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.TemplateModels
{
    public class TtlCategoryProductModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

		public string Url { get; set; }

		public string Thumbnail { get; set; }
	}
}
