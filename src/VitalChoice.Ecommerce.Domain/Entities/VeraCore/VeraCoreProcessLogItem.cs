using System;

namespace VitalChoice.Ecommerce.Domain.Entities.VeraCore
{
    public class VeraCoreProcessLogItem : Entity
    {
        public DateTime DateCreated { get; set; }

        public string FileName { get; set; }

        public DateTime FileDate { get; set; }

        public long FileSize { get; set; }
    }
}
