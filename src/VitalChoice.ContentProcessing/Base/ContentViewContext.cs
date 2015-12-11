using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Claims;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Base
{
    public class ContentViewContext
    {
        private readonly ExpandoObject _parameters;

        public ContentViewContext(IDictionary<string, object> parameters, ContentDataItem entity, ClaimsPrincipal user)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            _parameters = new ExpandoObject();
            parameters.CopyToDictionary(_parameters);
            BaseEntity = entity;
            User = user;
        }

        public dynamic Parameters => _parameters;

        public IDictionary<string, object> ParametersDictionary => _parameters as IDictionary<string, object>;

        public ContentDataItem BaseEntity { get; protected set; }
        public ClaimsPrincipal User { get; }
    }

    public class ContentViewContext<T> : ContentViewContext
        where T : ContentDataItem
    {
        public ContentViewContext(IDictionary<string, object> parameters, T entity, ClaimsPrincipal user) : base(parameters, entity, user)
        {
        }

        public T Entity
        {
            get { return (T) BaseEntity; }
            set { BaseEntity = value; }
        }
    }
}