using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;

namespace VitalChoice.Ecommerce.Domain.Dynamic
{
    public abstract class MappedObject: IModelType
    {
        protected MappedObject()
        {
            StatusCode = (int)RecordStatusCode.Active;
        }

        public int Id { get; set; }
        public int StatusCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEdited { get; set; }
        public int? IdEditedBy { get; set; }
        public Type ModelType { get; set; }
        public virtual int IdObjectType { get; set; }

        [JsonIgnore]
        public IDictionary<string, object> DictionaryData => DynamicData as IDictionary<string, object>;

        [JsonIgnore]
        public dynamic Data => DynamicData;

        public ExpandoObject DynamicData { get; set; } = new ExpandoObject();

        [JsonIgnore]
        public ICollection<ObjectHistoryLogItem> HistoryLogItems { get; set; }
    }
}