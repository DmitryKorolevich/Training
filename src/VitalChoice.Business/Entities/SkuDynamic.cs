using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Product;
using VitalChoice.DynamicData;

namespace VitalChoice.Business.Entities
{
    public class SkuDynamic : DynamicObject<Sku, ProductOptionValue, ProductOptionType>
    {

    }
}
