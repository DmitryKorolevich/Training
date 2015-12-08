using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Remotion.Linq.Parsing;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Ecommerce.Domain.Dynamic
{
    [DataContract]
    [JsonObject(MemberSerialization.OptOut)]
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
        [JsonIgnore]
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

        public T GetValueSafe<T>(string name)
        {
            object result;
            if (DictionaryData.TryGetValue(name, out result))
            {
                return result is T ? (T) result : default(T);
            }
            return default(T);
        }

        public T GetValue<T>(string name)
        {
            object result;
            if (DictionaryData.TryGetValue(name, out result))
            {
                if (result is T)
                    return (T) result;
                throw new ApiException($"Value type of {name} is <{result.GetType()}> but trying to cast to <{typeof (T)}>");
            }
            throw new ApiException($"Value {name} not found in <{GetType()}>");
        }

        [JsonIgnore]
        public IDictionary<string, object> DictionaryData => DynamicData as IDictionary<string, object>;

        [JsonIgnore]
        public dynamic Data => DynamicData;

        [JsonIgnore]
        public dynamic SafeData => _expandoData ?? (_expandoData = new SafeExpandoObject(DynamicData));

        private SafeExpandoObject _expandoData;

        public ExpandoObject DynamicData { get; set; }

        [JsonIgnore]
        public ICollection<ObjectHistoryLogItem> HistoryLogItems { get; set; }
    }
}