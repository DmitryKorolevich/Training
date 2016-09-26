using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Entities.InventorySkus
{
    public class SkuInventoryInfoImportItem : BaseImportItem
    {
        public SkuInventoryInfoImportItem()
        {
            SkuToInventorySkus = new List<SkuToInventorySku>();
        }

        [Required]
        [Display(Name = "SKU")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string SKU { get; set; }

        public int IdSku { get; set; }

        public int IdProduct { get; set; }

        [Required]
        [Display(Name = "Channel")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Channel { get; set; }

        public int IdChannel { get; set; }

        [Required]
        [Display(Name = "Assemble")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Assemble { get; set; }

        public bool AssembleBool { get; set; }

        [Display(Name = "Born Date")]
        public DateTime? BornDate { get; set; }

        [Display(Name = "Parts")]
        [MaxLength(BaseAppConstants.DEFAULT_BIG_TEXT_FIELD_MAX_SIZE)]
        public string Parts { get; set; }

        public ICollection<SkuToInventorySku> SkuToInventorySkus { get; set; }
    }
}
