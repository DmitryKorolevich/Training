using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public enum GCImportNotificationType
    {
        None = 1,
        StandartAdminEGiftEmail =2,
        ExpirationDateAdminEGiftEmail = 4,
    }
}