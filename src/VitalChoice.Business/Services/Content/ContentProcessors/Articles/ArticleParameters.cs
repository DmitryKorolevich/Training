using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.TypeConverters;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticleParameters
    {
        [ConvertWith(typeof(StringToIntConverter), 1)]
        public int Page { get; set; }

        [ConvertWith(typeof(StringToBoolConverter))]
        public bool Preview { get; set; }
        
        public string Url { get; set; }
    }
}
