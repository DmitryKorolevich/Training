using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class OrderGiftListImportItem : OrderBaseImportItem
    {
        [Display(Name = "Order Notes")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string OrderNotes { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Email { get; set; }

        [Display(Name = "Greeting")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string GiftMessage { get; set; }

        [Display(Name = "Ship Date")]
        public DateTime? ShipDelayDate { get; set; }               
    }
}
