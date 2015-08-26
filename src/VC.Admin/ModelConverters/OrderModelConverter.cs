using System;
using System.Collections.Generic;
using VC.Admin.Models.Order;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class OrderModelConverter : IModelToDynamicConverter<OrderManageModel, OrderDynamic>
    {
        public void DynamicToModel(OrderManageModel model, OrderDynamic dynamic)
        {
            if(dynamic.Discount!=null)
            {
                model.DiscountCode = dynamic.Discount.Code;
            }

            if(dynamic.GiftCertificates!=null && dynamic.GiftCertificates.Count>0)
            {
                if(model.GCs==null)
                {
                    model.GCs = new List<GCListItemModel>();
                }
                foreach(var item in dynamic.GiftCertificates)
                {
                    model.GCs.Add(new GCListItemModel(item.GiftCertificate));
                }
            }

            if (dynamic.Skus != null)
            {
                model.SkuOrdereds= new List<SkuOrderedManageModel>();
                foreach (var item in dynamic.Skus)
                {
                    model.SkuOrdereds.Add(new SkuOrderedManageModel(item));
                }
            }

            if(!model.ShipDelayType.HasValue)
            {
                model.ShipDelayType = 0;
            }
        }

        public void ModelToDynamic(OrderManageModel model, OrderDynamic dynamic)
        {
            if(!String.IsNullOrEmpty(model.DiscountCode))
            {
                dynamic.Discount = new DiscountDynamic();
                dynamic.Discount.Code = model.DiscountCode;
            }

            if(model.GCs!=null)
            {
                dynamic.GiftCertificates = new List<GiftCertificateInOrder>();
                foreach(var gc in model.GCs)
                {
                    GiftCertificateInOrder item = new GiftCertificateInOrder();
                    item.GiftCertificate = new GiftCertificate();
                    item.GiftCertificate.Code = gc.Code;
                    dynamic.GiftCertificates.Add(item);
                }
            }

            if (model.SkuOrdereds != null)
            {
                dynamic.Skus = new List<SkuOrdered>();
                foreach (var item in model.SkuOrdereds)
                {
                    dynamic.Skus.Add(item.Convert());
                }
            }

            if(dynamic.DictionaryData.ContainsKey("ShipDelayType") && (int?)dynamic.DictionaryData["ShipDelayType"] == 0)
            {
                dynamic.DictionaryData["ShipDelayType"] = null;
            }
        }
    }
}