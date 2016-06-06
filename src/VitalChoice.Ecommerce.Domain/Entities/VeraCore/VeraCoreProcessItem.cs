using System;

namespace VitalChoice.Ecommerce.Domain.Entities.VeraCore
{
    public class VeraCoreProcessItem : Entity
    {
        public int Attempt { get; set; }

        public VeraCoreProcessItemType? IdType { get; set; }

        public DateTime DateCreated { get; set; }

        public string FileName { get; set; }

        public DateTime FileDate { get; set; }

        public long FileSize { get; set; }

        public string Data { get; set; }
    }
}
