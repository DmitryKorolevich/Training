using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Ecommerce.Domain.Dynamic
{
    [DataContract]
    public abstract class MappedObject: IModelType
    {
        protected MappedObject()
        {
            StatusCode = (int)RecordStatusCode.Active;
            DynamicData = new ExpandoObject();
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int StatusCode { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public DateTime DateEdited { get; set; }
        [DataMember]
        public int? IdEditedBy { get; set; }

        public Type ModelType { get; set; }
        [DataMember]
        public virtual int IdObjectType { get; set; }

        [DataMember]
        internal List<KeyValuePair<string, object>> DynamicValues
        {
            get { return DynamicData.ToList(); }
            set
            {
                if (DynamicData == null)
                    DynamicData = new ExpandoObject();
                DictionaryData.AddRange(value);
            }
        }

        [JsonIgnore]
        public IDictionary<string, object> DictionaryData => DynamicData as IDictionary<string, object>;

        [JsonIgnore]
        public dynamic Data => DynamicData;

        public ExpandoObject DynamicData { get; set; }

        [JsonIgnore]
        public ICollection<ObjectHistoryLogItem> HistoryLogItems { get; set; }
    }
}