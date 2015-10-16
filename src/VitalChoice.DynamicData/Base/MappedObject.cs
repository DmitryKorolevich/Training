using System;
using System.Collections.Generic;
using System.Dynamic;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public abstract class MappedObject: IModelTypeContainer
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
        public Type ModelType { get; internal set; }
        public int? IdObjectType { get; set; }

        public IDictionary<string, object> DictionaryData => DynamicData as IDictionary<string, object>;

        public dynamic Data => DynamicData;

        protected internal ExpandoObject DynamicData { get; } = new ExpandoObject();

        public ICollection<ObjectHistoryLogItem> HistoryLogItems { get; set; }
    }
}