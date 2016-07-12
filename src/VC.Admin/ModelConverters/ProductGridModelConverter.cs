using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Admin.Models.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Admin.ModelConverters
{
    public class ProductGridModelConverter : BaseModelConverter<SkuListItemModel, SkuDynamic>
    {
        public override Task DynamicToModelAsync(SkuListItemModel model, SkuDynamic dynamic)
        {
            model.InStock = (ProductType) dynamic.Product.IdObjectType == ProductType.EGс ||
                            (ProductType) dynamic.Product.IdObjectType == ProductType.Gc ||
                            ((bool?) dynamic.SafeData.DisregardStock ?? false) || ((int?) dynamic.SafeData.Stock ?? 0) > 0;
            model.ProductName = dynamic.Product?.Name;
            model.DescriptionName =
                $"{dynamic.Product?.Name} {dynamic.Product?.SafeData.SubTitle ?? string.Empty} ({(int?) dynamic.SafeData.QTY ?? 0})";
            return TaskCache.CompletedTask;
        }

        public override Task ModelToDynamicAsync(SkuListItemModel model, SkuDynamic dynamic)
        {
            return TaskCache.CompletedTask;
        }
    }
}
