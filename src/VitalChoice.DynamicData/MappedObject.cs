using System;
using System.Collections.Generic;
using System.Dynamic;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData
{
    // ReSharper disable UnusedTypeParameter
    public abstract class MappedObject: IModelTypeContainer
    {
        public int Id { get; set; }
        public RecordStatusCode StatusCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEdited { get; set; }
        public int? IdEditedBy { get; set; }
        public Type ModelType { get; internal set; }

        public IDictionary<string, object> DictionaryData => DynamicData as IDictionary<string, object>;

        public dynamic Data => DynamicData;

        protected internal ExpandoObject DynamicData { get; } = new ExpandoObject();
    }
}