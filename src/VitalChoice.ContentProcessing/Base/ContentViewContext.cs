using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Claims;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.Base;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Templates.Strings;

namespace VitalChoice.ContentProcessing.Base
{
    public class ContentViewContext
    {
        private readonly ExpandoObject _parameters;
        private readonly ExStringBuilder _scriptStringBuilder = new ExStringBuilder();
        private readonly ExStringBuilder _socialMetaStringBuilder = new ExStringBuilder();

        public ContentViewContext(IDictionary<string, object> parameters, ContentDataItem entity, ClaimsPrincipal user,
            ActionContext actionContext)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            _parameters = new ExpandoObject();
            parameters.CopyToDictionary(_parameters);
            BaseEntity = entity;
            User = user;
            ActionContext = actionContext;
            AbsoluteUrl = actionContext.HttpContext.Request.GetDisplayUrl();
            CommandOptions=new ViewContentCommandOptions();
        }

        public dynamic Parameters => _parameters;

        public IDictionary<string, object> ParametersDictionary => _parameters as IDictionary<string, object>;

        public ContentDataItem BaseEntity { get; protected set; }
        public ClaimsPrincipal User { get; }
        public ActionContext ActionContext { get; }

        public string Scripts => _scriptStringBuilder.ToString();

        public string AbsoluteUrl { get; }

        public void AppendScript(string scripts)
        {
            _scriptStringBuilder.Append(scripts);
        }

        public string SocialMeta => _socialMetaStringBuilder.ToString();

        public void AppendSocialMeta(string meta)
        {
            _socialMetaStringBuilder.Append(meta);
        }

        public ViewContentCommandOptions CommandOptions { get; set; }
    }

    public class ContentViewContext<T> : ContentViewContext
        where T : ContentDataItem
    {
        public ContentViewContext(IDictionary<string, object> parameters, T entity, ClaimsPrincipal user, ActionContext actionContext)
            : base(parameters, entity, user, actionContext)
        {
        }

        public T Entity
        {
            get { return (T) BaseEntity; }
            set { BaseEntity = value; }
        }
    }
}