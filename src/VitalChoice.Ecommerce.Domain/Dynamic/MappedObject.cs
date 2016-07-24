using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
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
            StatusCode = (int) RecordStatusCode.Active;
            UnsafeData = new UnsafeDynamicObject();
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
            get { return UnsafeData.Dictionary.ToList(); }
            set
            {
                if (UnsafeData == null)
                    UnsafeData = new UnsafeDynamicObject();
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
                throw new ApiException($"Value type of {name} is <{result.GetType()}> cannot be cast to <{typeof (T)}>");
            }
            throw new ApiException($"Value {name} not found in <{GetType()}>");
        }

        [JsonIgnore]
        public IDictionary<string, object> DictionaryData => UnsafeData.Dictionary;

        [JsonIgnore]
        public dynamic Data => _unsafeData;

        [JsonIgnore]
        public dynamic SafeData => _safeData;

        private SafeDynamicObject _safeData;
        private UnsafeDynamicObject _unsafeData;
        
        public UnsafeDynamicObject UnsafeData
        {
            get { return _unsafeData; }
            set
            {
                _unsafeData = value;
                _safeData = new SafeDynamicObject(value);
            }
        }

        [JsonIgnore]
        public ICollection<ObjectHistoryLogItem> HistoryLogItems { get; set; }
    }
}