using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Entities.Orders
{
    public class OrderBaseImportItem
    {
        public int RowNumber { get; set; }

        [Map]
        [Required]
        [Display(Name = "First Name")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string FirstName { get; set; }

        [Map]
        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string LastName { get; set; }

        [Map]
        [Display(Name = "Company")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Company { get; set; }

        [Map]
        [Required]
        [Display(Name = "Address 1")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address1 { get; set; }

        [Map]
        [Display(Name = "Address 2")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Address2 { get; set; }

        [Map]
        [Required]
        [Display(Name = "City")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string State { get; set; }

        [Map]
        public int? IdState { get; set; }

        [Required]
        [Display(Name = "Country")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Country { get; set; }

        [Map]
        public int? IdCountry { get; set; }

        [Map]
        [Required]
        [Display(Name = "Postal Code")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Zip { get; set; }

        [Map]
        [Required]
        [Display(Name = "Phone")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Phone { get; set; }

        [Map]
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        public string Email { get; set; }

        public ICollection<OrderSkuImportItem> Skus { get; set; }

        public ICollection<MessageInfo> ErrorMessages { get; set; }

        public virtual void SetFields(OrderDynamic order)
        {
            order.DateCreated = DateTime.Now;
            order.StatusCode = (int)RecordStatusCode.Active;
            order.PaymentMethod = new OrderPaymentMethodDynamic()
            {
                IdObjectType = (int)PaymentMethodType.NoCharge,
            };
            order.Skus = this?.Skus.Select(p => new SkuOrdered()
            {
                Sku = new SkuDynamic() { Code = p.SKU },
                Quantity = p.QTY,
            }).ToList();
        }
    }
}
