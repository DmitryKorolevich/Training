using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Product;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces; 
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Domain.Constants;

namespace VC.Admin.Models.Product
{
using VitalChoice.DynamicData.Entities;

    public class SKUManageModel : Model<SkuDynamic, IMode>, IModelToDynamic<SkuDynamic>
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.SKU)]
        public string Name { get; set; }

        public bool Active { get; set; }

        public bool Hidden { get; set; }

        public decimal RetailPrice { get; set; }

        public decimal WholesalePrice { get; set; }

        [Map]
        public int? Stock { get; set; }

        [Map]
        public bool DisregardStock { get; set; }

        [Map]
        public bool DisallowSingle { get; set; }

        [Map]
        public bool NonDiscountable { get; set; }

        [Map]
        public bool OrphanType { get; set; }

        [Map]
        public bool AutoShipProduct { get; set; }

        [Map]
        public double OffPercent { get; set; }

        [Map]
        public int Seller { get; set; }

        [Map]
        public bool HideFromDataFeed { get; set; }

        [Map]
        public bool AutoShipFrequency1 { get; set; }

        [Map]
        public bool AutoShipFrequency2 { get; set; }

        [Map]
        public bool AutoShipFrequency3 { get; set; }

        [Map]
        public bool AutoShipFrequency6 { get; set; }

        public SKUManageModel()
        {
        }

        public override SkuDynamic Convert()
        {
            SkuDynamic toReturn = new SkuDynamic();
            toReturn.FromModel<SKUManageModel, SkuDynamic>(this);

            return toReturn;
        }

        public void FillDynamic(SkuDynamic dynamicObject)
        {
            dynamicObject.Id = Id;
            dynamicObject.Code = Name;
            dynamicObject.StatusCode = Active ? RecordStatusCode.Active : RecordStatusCode.NotActive;
            dynamicObject.Hidden = Hidden;
            dynamicObject.Price = RetailPrice;
            dynamicObject.WholesalePrice = WholesalePrice;
        }

        public void FillSelfFrom(SkuDynamic dynamicObject)
        {
            Id = dynamicObject.Id;
            Name = dynamicObject.Code;
            Active = dynamicObject.StatusCode == RecordStatusCode.Active;
            Hidden = dynamicObject.Hidden;
            RetailPrice = dynamicObject.Price;
            WholesalePrice = dynamicObject.WholesalePrice;
        }
    }
}