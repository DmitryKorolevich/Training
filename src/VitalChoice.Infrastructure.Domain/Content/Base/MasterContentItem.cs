using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Content.Base
{
    public class MasterContentItem : LogEntity
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public int TypeId { get; set; }

        public ContentTypeEntity Type { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
        
        public ApplicationUser User { get; set; }

        public ICollection<MasterContentItemToContentProcessor> MasterContentItemToContentProcessors { get; set; }
    }
}