using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VC.Admin.Models.Product
{
    public class SkuWithStatisticListItemModel : SkuListItemModel
    {
        public int Ordered { get; set; }

        public SkuWithStatisticListItemModel(VSku item, int ordered) : base(item)
        {
            if(item!=null)
            {
                Ordered = ordered;
            }
        }
    }
}