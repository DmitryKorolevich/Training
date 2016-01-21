using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class OrderBaseImportItem
    {
        public int RowNumber { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string LastName { get; set; }

        [Display(Name = "Company")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Company { get; set; }

        [Required]
        [Display(Name = "Address 1")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address { get; set; }

        [Display(Name = "Address 2")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "City")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string State { get; set; }

        public int? IdState { get; set; }

        [Required]
        [Display(Name = "Country")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Country { get; set; }

        public int? IdCountry { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Zip { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Phone { get; set; }

        public ICollection<OrderSkuImportItem> Skus { get; set; }

        public ICollection<MessageInfo> ErrorMessages { get; set; }
    }
}
