using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces; 
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Localization.Groups;

namespace VC.Admin.Models.Product
{
    public class SKUManageModel : BaseModel 
    {
        [Map]
        public int Id { get; set; }

        [Map("Code")]
        [Localized(GeneralFieldNames.SKU)]
        public string Name { get; set; }

        public bool Active { get; set; }

        [Map]
        public bool Hidden { get; set; }

        [Map("Price")]
        public decimal RetailPrice { get; set; }

        [Map]
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
        public double? OffPercent { get; set; }

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
    }
}