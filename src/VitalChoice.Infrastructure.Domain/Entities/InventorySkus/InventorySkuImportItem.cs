using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Entities.InventorySkus
{
    public class InventorySkuImportItem : BaseImportItem
    {
        [Required]
        [Display(Name = "Code")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Active")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Active { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Source")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Source { get; set; }

        [Required]
        [Display(Name = "Inv Qty")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string InvQty { get; set; }

        public int InvQtyInt { get; set; }

        [Required]
        [Display(Name = "Inv UOM")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string InvUOM { get; set; }

        [Required]
        [Display(Name = "Inv Unit Amt")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string InvUnitAmt { get; set; }

        public decimal InvUnitAmtDec { get; set; }

        [Required]
        [Display(Name = "Purchase UOM")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string PurchaseUOM { get; set; }

        [Required]
        [Display(Name = "UOM Qty")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string UOMQty { get; set; }

        public int UOMQtyInt { get; set; }

        [Required]
        [Display(Name = "Parts Category")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string PartsCategory { get; set; }
    }
}
