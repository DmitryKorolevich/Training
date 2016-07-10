using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public sealed class LogDataItemTableEntity : TableEntity
    {
        public LogDataItemTableEntity()
        {
            
        }

        public LogDataItemTableEntity(int idObjectType, object idObject, DateTime dateCreated)
        {
            PartitionKey = idObject.ToString();
            RowKey = $"{idObjectType}{dateCreated.ToBinary().ToString("x16")}";
            IdObjectType = idObjectType;
            DateCreated = dateCreated;
        }

        public int IdObjectType { get; set; }

        public int IdObjectStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        [IgnoreProperty]
        public string Data { get; set; }

        public string OptionalData { get; set; }
    }
}