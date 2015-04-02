using System;

namespace VitalChoice.Domain.Entities.Content
{
    public class ContentTypeEntity : Entity
    {
        public string Name { get; set; }

        public int? DefaultMasterContentItemId { get; set; }
    }
}