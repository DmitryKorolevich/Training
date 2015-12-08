using System;
using System.Collections.Generic;
using System.Dynamic;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Base
{
    public class ContentViewContext
    {
        private readonly ExpandoObject _parameters;

        public ContentViewContext(IDictionary<string, object> parameters, ContentDataItem entity)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            _parameters = new ExpandoObject();
            parameters.CopyToDictionary(_parameters);
            BaseEntity = entity;
        }

        public dynamic Parameters => _parameters;

        public ContentDataItem BaseEntity { get; protected set; }
    }

    public class ContentViewContext<T> : ContentViewContext
        where T : ContentDataItem
    {
        public ContentViewContext(IDictionary<string, object> parameters, T entity) : base(parameters, entity)
        {
        }

        public T Entity
        {
            get { return (T) BaseEntity; }
            set { BaseEntity = value; }
        }
    }
}