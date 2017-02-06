using System;
using System.Collections.Generic;
using VC.Admin.Validators.Order;
using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Orders
{
    [ApiValidator(typeof(OrderReviewRuleManageModelValidator))]
    public class OrderReviewRuleManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        [Map]
        public ApplyType ApplyType { get; set; }

        [Map]
        public decimal? MinOrderTotal { get; set; }

        [Map]
        public bool Guest { get; set; }

        [Map]
        public bool FirstTimeOrder { get; set; }

        [Map]
        public string DeliveryInstructionForSearch { get; set; }

        [Map]
        public string ZipForSearch { get; set; }

        [Map]
        public string SkuForSearch { get; set; }

        [Map]
        public bool CompareNames { get; set; }

        [Map]
        public CompareType? CompareNamesType { get; set; }

        [Map]
        public bool CompareAddresses { get; set; }

        [Map]
        public CompareType? CompareAddressesType { get; set; }

        [Map]
        public bool ReshipsRefundsCheck { get; set; }

        [Map]
        public int? ReshipsRefundsQTY { get; set; }

        [Map]
        public OrderType? ReshipsRefundsCheckType { get; set; }

        [Map]
        public int? ReshipsRefundsMonthCount { get; set; }

        public OrderReviewRuleManageModel()
        {
        }
    }
}