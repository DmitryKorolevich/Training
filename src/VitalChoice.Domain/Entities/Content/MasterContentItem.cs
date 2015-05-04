using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.Content
{
    public class MasterContentItem : Entity
    {
        public string Name { get; set; }
        public string Template { get; set; }
        public int TypeId { get; set; }
        public ContentTypeEntity Type { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public RecordStatusCode StatusCode { get; set; }
        public int? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<MasterContentItemToContentProcessor> MasterContentItemToContentProcessors { get; set; }
    }
}