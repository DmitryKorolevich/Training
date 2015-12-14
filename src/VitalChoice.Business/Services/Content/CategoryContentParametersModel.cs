using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.DynamicData.TypeConverters;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Business.Services.Content
{
    public class CategoryContentParametersModel : ContentParametersModel
    {
        [ConvertWith(typeof(StringToIntConverter), 1)]
        public int Page { get; set; }
    }
}
